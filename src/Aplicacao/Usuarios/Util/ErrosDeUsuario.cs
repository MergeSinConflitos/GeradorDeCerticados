
using FluentResults;
using GeradorDeCertificados.Aplicacao.Compartilhado;

namespace GeradorDeCertificados.Aplicacao.Usuarios.Util;

public static class ErrosDeUsuario
{
    public static Error CredenciaisInvalidas()
        => TipoErro.NaoAutenticado.ObterMetadados(
            string.Empty,
            "Email ou senha inválidos."
        );

    public static Error EmailInvalido()
        => new Error("O email informado é inválido.")
            .WithMetadata("Codigo", "Usuario.EmailInvalido");

    public static Error EmailJaCadastrado()
        => new Error("O email informado já está cadastrado.")
            .WithMetadata("Codigo", "Usuario.EmailJaCadastrado");

    public static Error SenhaMuitoCurta()
        => new Error("A senha deve possuir no mínimo 8 caracteres.")
            .WithMetadata("Codigo", "Usuario.SenhaMuitoCurta");

    public static Error SenhaSemDigito()
        => new Error("A senha deve conter pelo menos um dígito.")
            .WithMetadata("Codigo", "Usuario.SenhaSemDigito");

    public static Error SenhaSemCaractereNaoAlfanumerico()
        => new Error(
            "A senha deve conter pelo menos um caractere não alfanumérico."
        )
        .WithMetadata(
            "Codigo",
            "Usuario.SenhaSemCaractereNaoAlfanumerico"
        );

    internal static IError ConflitoDeIdentidade(
        string mensagem
    )
        => TipoErro.Conflito.ObterMetadados(
            string.Empty,
            mensagem
        );

    internal static IError ValidacaoDeIdentidade(
        string campo,
        string mensagem
    )
        => TipoErro.Validacao.ObterMetadados(
            campo,
            mensagem
        );

    internal static IError UsuarioNaoEncontrado()
        => TipoErro.NaoEncontrado.ObterMetadados(
            string.Empty,
            "Usuário não encontrado."
        );
}
