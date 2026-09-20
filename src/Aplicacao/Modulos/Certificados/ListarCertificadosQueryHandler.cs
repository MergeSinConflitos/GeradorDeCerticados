using FluentResults;
using MediatR;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.DTOs;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.Util;
using GeradorDeCertificados.Dominio.Modulos.Certificados;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados;

public sealed record ListarCertificadosQuery(
    Guid CursoId
) : IRequest<Result<List<CertificadoDto>>>;

public sealed class ListarCertificadosQueryHandler(
    IRepositorioCertificado repositorioCertificado
) : IRequestHandler<
    ListarCertificadosQuery,
    Result<List<CertificadoDto>>>
{
    public async Task<Result<List<CertificadoDto>>> Handle(
        ListarCertificadosQuery request,
        CancellationToken cancellationToken)
    {
        List<Certificado> certificados =
            await repositorioCertificado.ListarPorCursoIdAsync(
                request.CursoId,
                cancellationToken);

        List<CertificadoDto> certificadosDto =
            certificados
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

        return Result.Ok(certificadosDto);
    }
}