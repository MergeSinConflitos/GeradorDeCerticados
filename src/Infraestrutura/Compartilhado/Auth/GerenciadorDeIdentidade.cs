using GeradorDeCertificados.Dominio.Compartilhado.Auth;
using Microsoft.AspNetCore.Identity;

namespace GeradorDeCertificados.Infraestrutura.Compartilhado.Auth;

public sealed class GerenciadorDeIdentidade(
    UserManager<IdentityUser<Guid>> userManager
) : IGerenciadorDeIdentidade
{
    public async Task<UsuarioDto> CadastrarAsync(
        Guid usuarioId,
        string email,
        string senha
    )
    {
        var usuario = new IdentityUser<Guid>
        {
            Id = usuarioId,
            Email = email.Trim(),
            UserName = email.Trim()
        };

        var resultadoUsuario = await userManager.CreateAsync(usuario, senha);

        if (!resultadoUsuario.Succeeded)
            throw CriarErro(resultadoUsuario);

        return new UsuarioDto(
            usuario.Id,
            usuario.Email!
        );
    }

    public async Task<UsuarioDto?> ObterPorIdAsync(
        Guid usuarioId
    )
    {
        var usuario = await userManager.FindByIdAsync(
            usuarioId.ToString()
        );

        if (usuario is null)
            return null;

        return new UsuarioDto(
            usuario.Id,
            usuario.Email!
        );
    }

    public async Task<UsuarioDto?> ChecarValidadeDeSenhaAsync(
        string email,
        string senha
    )
    {
        var usuario = await userManager.FindByEmailAsync(email);

        if (usuario is null || await userManager.IsLockedOutAsync(usuario))
            return null;

        if (!await userManager.CheckPasswordAsync(usuario, senha))
        {
            await userManager.AccessFailedAsync(usuario);

            return null;
        }

        if (usuario.AccessFailedCount > 0)
            await userManager.ResetAccessFailedCountAsync(usuario);

        return new UsuarioDto(
            usuario.Id,
            usuario.Email!
        );
    }

    public async Task ExcluirAsync(Guid usuarioId)
    {
        var usuario = await userManager.FindByIdAsync(usuarioId.ToString());

        if (usuario is not null)
            await userManager.DeleteAsync(usuario);
    }

    private static Exception CriarErro(IdentityResult resultado)
    {
        var erro = resultado.Errors.First();

        //Mapeia os erros do identity(por padrão em ingles)
        //para conseguir entender mensagens em portugues
        return erro.Code switch
        {
            "DuplicateEmail" or "DuplicateUserName"
                => new ConflitoDeIdentidadeException(
                    "Já existe um usuário cadastrado com este email."
                ),

            "InvalidEmail"
                => new ValidacaoDeIdentidadeException(
                    "Email",
                    "O email informado é inválido."
                ),

            "PasswordTooShort"
                => new ValidacaoDeIdentidadeException(
                    "Senha",
                    "A senha deve possuir no mínimo 8 caracteres."
                ),

            "PasswordRequiresDigit"
                => new ValidacaoDeIdentidadeException(
                    "Senha",
                    "A senha deve conter pelo menos um dígito."
                ),

            "PasswordRequiresNonAlphanumeric"
                => new ValidacaoDeIdentidadeException(
                    "Senha",
                    "A senha deve conter pelo menos um caractere não alfanumérico."
                ),

            _ => new ValidacaoDeIdentidadeException(
                "Usuario",
                erro.Description
            )
        };
    }


}