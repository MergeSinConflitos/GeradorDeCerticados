
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
