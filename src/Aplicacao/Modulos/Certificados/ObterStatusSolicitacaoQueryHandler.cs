using FluentResults;
using MediatR;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.DTOs;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.Util;
using GeradorDeCertificados.Dominio.Modulos.Certificados;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados;

public sealed record ObterStatusSolicitacaoQuery(
    Guid CursoId
) : IRequest<Result<StatusProcessamento>>;

public sealed class ObterStatusSolicitacaoQueryHandler(
    IRepositorioSolicitacaoCertificado repositorioSolicitacaoCertificado
) : IRequestHandler<
    ObterStatusSolicitacaoQuery,
    Result<StatusProcessamento>>
{
    public async Task<Result<StatusProcessamento>> Handle(
        ObterStatusSolicitacaoQuery request,
        CancellationToken cancellationToken)
    {
        SolicitacaoCertificado? solicitacao =
            await repositorioSolicitacaoCertificado
                .SelecionarPorCursoIdAsync(
                    request.CursoId,
                    cancellationToken);

        if (solicitacao is null)
        {
            return Result.Fail(
                ErrosDeCertificado.NaoEncontrado(request.CursoId));
        }

        return Result.Ok(solicitacao.Status);
    }
}