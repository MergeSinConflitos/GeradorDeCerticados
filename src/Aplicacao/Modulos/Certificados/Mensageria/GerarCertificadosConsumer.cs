using MassTransit;
using Microsoft.Extensions.Logging;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.Mensageria;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados;

public sealed class GerarCertificadosConsumer(
    IRepositorioSolicitacaoCertificado repositorioSolicitacao,
    IRepositorioCurso repositorioCurso,
    GeradorPdfCertificado geradorPdf,
    IArmazenadorDeArquivo armazenador,
    IEmpacotadorDeArquivo empacotador,
    ILogger<GerarCertificadosConsumer> logger
) : IConsumer<GerarCertificadosMessage>
{
    public async Task Consume(ConsumeContext<GerarCertificadosMessage> context)
    {
        var cancellationToken = context.CancellationToken;
        var mensagem = context.Message;

        logger.LogInformation(
            "Iniciando processamento da solicitação {SolicitacaoId}",
            mensagem.SolicitacaoCertificadoId);

        var solicitacao = await repositorioSolicitacao.SelecionarPorIdAsync(
            mensagem.SolicitacaoCertificadoId, cancellationToken);

        if (solicitacao is null)
        {
            logger.LogWarning(
                "Solicitação {SolicitacaoId} não foi encontrada. Mensagem descartada.",
                mensagem.SolicitacaoCertificadoId);
            return;
        }

        var curso = await repositorioCurso.SelecionarPorIdAsync(
            solicitacao.CursoId, cancellationToken);

        if (curso is null)
        {
            logger.LogWarning(
                "Curso {CursoId} da solicitação {SolicitacaoId} não foi encontrado. Marcando como falha.",
                solicitacao.CursoId, solicitacao.Id);

            solicitacao.MarcarComoFalha();
            await repositorioSolicitacao.EditarAsync(solicitacao.Id, solicitacao, cancellationToken);
            return;
        }

        solicitacao.IniciarProcessamento();
        await repositorioSolicitacao.EditarAsync(solicitacao.Id, solicitacao, cancellationToken);

        var arquivosParaZip = new List<ArquivoParaEmpacotar>();

        foreach (var certificado in solicitacao.Certificados)
        {
            try
            {
                var pdfBytes = geradorPdf.Gerar(new DetalhesCertificadoDto(
                    certificado.NomeAluno,
                    curso.Nome,
                    curso.CargaHoraria,
                    curso.DataConclusao
                ));

                var nomeArquivo = $"certificado-{certificado.Id}.pdf";
                var caminho = await armazenador.SalvarAsync(nomeArquivo, pdfBytes, cancellationToken);

                certificado.MarcarComoGerado(caminho, DateOnly.FromDateTime(DateTime.UtcNow));
                arquivosParaZip.Add(new ArquivoParaEmpacotar(nomeArquivo, pdfBytes));

                logger.LogInformation(
                    "Certificado {CertificadoId} gerado para o aluno {NomeAluno}",
                    certificado.Id, certificado.NomeAluno);
            }
            catch (Exception ex)
            {
                certificado.MarcarComoFalha();

                logger.LogError(ex,
                    "Falha ao gerar certificado {CertificadoId} para o aluno {NomeAluno}",
                    certificado.Id, certificado.NomeAluno);
            }
        }

        await repositorioSolicitacao.EditarAsync(solicitacao.Id, solicitacao, cancellationToken);

        solicitacao.IniciarGeracaoZip();
        await repositorioSolicitacao.EditarAsync(solicitacao.Id, solicitacao, cancellationToken);

        var zipBytes = empacotador.Empacotar(arquivosParaZip);
        var caminhoZip = await armazenador.SalvarAsync($"certificados-{solicitacao.Id}.zip", zipBytes, cancellationToken);

        solicitacao.Concluir(caminhoZip);
        await repositorioSolicitacao.EditarAsync(solicitacao.Id, solicitacao, cancellationToken);

        logger.LogInformation(
            "Solicitação {SolicitacaoId} concluída. ZIP salvo em {CaminhoZip}",
            solicitacao.Id, caminhoZip);
    }
}