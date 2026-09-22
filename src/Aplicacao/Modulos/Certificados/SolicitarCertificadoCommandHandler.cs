using FluentResults;
using MassTransit;
using MediatR;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.DTOs;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.Util;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using GeradorDeCertificados.Dominio.Compartilhado;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.Mensageria;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados;

public sealed record SolicitarCertificadosCommand(
    Guid CursoId,
    List<string> NomesAlunos
) : IRequest<Result<SolicitacaoCertificadoDto>>;

public sealed class SolicitarCertificadosCommandHandler(
    IRepositorioCurso repositorioCurso,
    IRepositorioSolicitacaoCertificado repositorioSolicitacaoCertificado,
    IPublishEndpoint publishEndpoint
) : IRequestHandler<
    SolicitarCertificadosCommand,
    Result<SolicitacaoCertificadoDto>>
{
    public async Task<Result<SolicitacaoCertificadoDto>> Handle(
        SolicitarCertificadosCommand command,
        CancellationToken cancellationToken)
    {
        Curso? curso = await repositorioCurso.SelecionarPorIdAsync(
            command.CursoId,
            cancellationToken);

        if (curso is null)
        {
            return Result.Fail(
                ErrosDeCertificado.NaoEncontrado(command.CursoId));
        }

        bool processamentoEmAndamento =
            await repositorioSolicitacaoCertificado
                .ExisteProcessamentoEmAndamentoAsync(
                    command.CursoId,
                    cancellationToken);

        if (processamentoEmAndamento)
        {
            return Result.Fail(
                ErrosDeCertificado.ProcessamentoEmAndamento());
        }

        Guid solicitacaoId = Guid.CreateVersion7();

        List<Certificado> certificados = command.NomesAlunos
            .Select(nomeAluno =>
                new Certificado(
                    Guid.CreateVersion7(),
                    command.CursoId,
                    solicitacaoId,
                    nomeAluno))
            .ToList();

        SolicitacaoCertificado solicitacao =
            new(
                solicitacaoId,
                command.CursoId,
                certificados);


        IReadOnlyList<ErroValidacao> erros =
            solicitacao.Validar();

        if (erros.Count > 0)
        {
            return Result.Fail(
                ErrosDeCertificado.Validacao(erros));
        }

        await repositorioSolicitacaoCertificado.CadastrarAsync(
            solicitacao,
            cancellationToken);

        await publishEndpoint.Publish(
            new GerarCertificadosMessage(solicitacao.Id),
            cancellationToken);

        return Result.Ok(MapearParaDto(solicitacao));
    }

    private static SolicitacaoCertificadoDto MapearParaDto(
        SolicitacaoCertificado solicitacao)
    {
        List<CertificadoDto> certificados =
            solicitacao.Certificados
                .Select(certificado =>
                    new CertificadoDto(
                        certificado.Id,
                        certificado.CursoId,
                        certificado.SolicitacaoCertificadoId,
                        certificado.NomeAluno,
                        certificado.CaminhoArquivo,
                        certificado.DataDeGeracao,
                        certificado.Status))
                .ToList();

        return new SolicitacaoCertificadoDto(
            solicitacao.Id,
            solicitacao.CursoId,
            solicitacao.CaminhoZip,
            solicitacao.DataDeCriacao,
            solicitacao.DataDeConclusao,
            certificados);
    }
}