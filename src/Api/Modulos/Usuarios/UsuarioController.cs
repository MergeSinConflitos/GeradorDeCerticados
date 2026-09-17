using GeradorDeCertificados.Api.Compartilhado.Http;
using GeradorDeCertificados.Aplicacao.Compartilhado;
using GeradorDeCertificados.Aplicacao.Modulos.Usuarios;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static GeradorDeCertificados.Api.Modulos.Usuarios.UsuarioContracts;

namespace GeradorDeCertificados.Api.Modulos.Usuarios;

[ApiController]
[Route("auth")]
public sealed class UsuariosController(
    IMediator mediator
) : ControllerBase
{
    [Authorize]
    [HttpGet("{usuarioId:guid}")]
    [ProducesResponseType<UsuarioResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<UsuarioResponse>> ObterPorId(
        Guid usuarioId,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(
            new ObterUsuarioPorIdQuery(usuarioId),
            cancellationToken
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return Ok(new UsuarioResponse(
            resultado.Value.Id,
            resultado.Value.Email
        ));
    }

    [AllowAnonymous]
    [HttpPost("cadastro")]
    [ProducesResponseType<CadastrarUsuarioResponse>(
        StatusCodes.Status201Created
    )]
    public async Task<ActionResult<CadastrarUsuarioResponse>> Cadastrar(
        CadastrarUsuarioRequest request,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(
            new CadastrarUsuarioCommand(
                request.Email,
                request.Senha
            ),
            cancellationToken
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { usuarioId = resultado.Value },
            new CadastrarUsuarioResponse(
                resultado.Value,
                request.Email
            )
        );
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<AutenticarUsuarioResponse>(
        StatusCodes.Status200OK
    )]
    public async Task<ActionResult<AutenticarUsuarioResponse>> Autenticar(
        AutenticarUsuarioRequest request,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(
            new AutenticarUsuarioCommand(
                request.Email,
                request.Senha
            ),
            cancellationToken
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        var accessTokenDoUsuario = resultado.Value;

        return Ok(new AutenticarUsuarioResponse(
            accessTokenDoUsuario.UsuarioId,
            accessTokenDoUsuario.Token,
            accessTokenDoUsuario.DataExpiracaoEmUtc
        ));
    }
}