
using GeradorDeCertificados.Dominio.Compartilhado.Auth;
using FluentResults;
using MediatR;
using GeradorDeCertificados.Aplicacao.Usuarios.Util;

namespace GeradorDeCertificados.Aplicacao.Modulos.Usuarios;

public sealed record ObterUsuarioPorIdQuery(
    Guid UsuarioId
) : IRequest<Result<UsuarioDto>>;

public sealed class ObterUsuarioPorIdQueryHandler(
    IGerenciadorDeIdentidade gerenciadorDeIdentidade
) : IRequestHandler<ObterUsuarioPorIdQuery, Result<UsuarioDto>>
{
    public async Task<Result<UsuarioDto>> Handle(
        ObterUsuarioPorIdQuery request,
        CancellationToken cancellationToken = default
    )
    {
        UsuarioDto? usuario =
            await gerenciadorDeIdentidade.ObterPorIdAsync(
                request.UsuarioId
            );

        if (usuario is null)
            return Result.Fail(
                ErrosDeUsuario.UsuarioNaoEncontrado()
            );

        return Result.Ok(usuario);
    }
}

