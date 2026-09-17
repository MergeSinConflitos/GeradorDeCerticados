namespace GeradorDeCertificados.Api.Modulos.Usuarios;

public sealed class UsuarioContracts
{
    public sealed record CadastrarUsuarioRequest(
        string Email,
        string Senha
    );

    public sealed record CadastrarUsuarioResponse(
        Guid Id,
        string Email
    );

    public sealed record AutenticarUsuarioRequest(
        string Email,
        string Senha
    );

    public sealed record AutenticarUsuarioResponse(
        Guid UsuarioId,
        string AccessToken,
        DateTime DataExpiracaoEmUtc
    );

    public sealed record UsuarioResponse(
        Guid Id,
        string Email
    );
}