using FluentResults;
using MediatR;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.DTOs;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.Util;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados;

public sealed record SolicitarCertificadosCommand(
    Guid CursoId,
    List<string> NomesAlunos
) : IRequest<Result<SolicitacaoCertificadoDto>>;

public sealed class SolicitarCertificadosCommandHandler(
    IRepositorioCurso repositorioCurso,
    IRepositorioSolicitacaoCertificado repositorioSolicitacaoCertificado
) : IRequestHandler<
    SolicitarCertificadosCommand,
    Result<SolicitacaoCertificadoDto>>
{
    public async Task<Result<SolicitacaoCertificadoDto>> Handle(
        SolicitarCertificadosCommand request,
        CancellationToken cancellationToken)
    {
        Curso? curso = await repositorioCurso.SelecionarPorIdAsync(
            request.CursoId,
            cancellationToken);

        if (curso is null)
        {
            return Result.Fail(
                ErrosDeCertificado.NaoEncontrado(request.CursoId));
        }

        bool processamentoEmAndamento =
            await repositorioSolicitacaoCertificado
                .ExisteProcessamentoEmAndamentoAsync(
                    request.CursoId,
                    cancellationToken);

        if (processamentoEmAndamento)
        {
            return Result.Fail(
                ErrosDeCertificado.ProcessamentoEmAndamento());
        }

        Guid solicitacaoId = Guid.CreateVersion7();

        List<Certificado> certificados = request.NomesAlunos
            .Select(nomeAluno =>
                new Certificado(
                    Guid.CreateVersion7(),
                    request.CursoId,
                    solicitacaoId,
                    nomeAluno))
            .ToList();

        SolicitacaoCertificado solicitacao =
            new(
                solicitacaoId,
                request.CursoId,
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

        return Result.Ok(
            MapearParaDto(solicitacao));
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