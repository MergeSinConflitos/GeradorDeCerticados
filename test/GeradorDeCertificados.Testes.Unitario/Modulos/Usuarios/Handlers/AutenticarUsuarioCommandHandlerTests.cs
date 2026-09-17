
using FluentResults;
using GeradorDeCertificados.Aplicacao.Compartilhado;
using GeradorDeCertificados.Aplicacao.Usuarios;
using GeradorDeCertificados.Dominio.Compartilhado.Auth;
using Moq;

namespace GeradorDeCertificados.Testes.Unitario.Modulos.Usuarios.Handlers;

[TestClass]
public sealed class AutenticarUsuarioCommandHandlerTests
{
    [TestMethod]
    public async Task DeveAutenticar_QuandoCredenciaisForemValidas()
    {
        // Arrange
        Guid usuarioId = Guid.CreateVersion7();
        string email = "usuario@email.com";
        string senha = "Senha@123";
        string token = "token-jwt";

        var usuario = new UsuarioDto(
            usuarioId,
            email
        );

        var accessToken = new AccessToken(
            token,
            DateTime.UtcNow.AddHours(1)
        );

        var gerenciadorDeIdentidade =
            new Mock<IGerenciadorDeIdentidade>();

        var emissorDeTokens =
            new Mock<IEmissorDeTokens>();

        gerenciadorDeIdentidade
            .Setup(g => g.ChecarValidadeDeSenhaAsync(
                email,
                senha
            ))
            .ReturnsAsync(usuario);

        emissorDeTokens
            .Setup(e => e.CriarToken(
                usuarioId,
                email
            ))
            .Returns(accessToken);

        var handler = new AutenticarUsuarioCommandHandler(
            gerenciadorDeIdentidade.Object,
            emissorDeTokens.Object
        );

        var command = new AutenticarUsuarioCommand(
            email,
            senha
        );

        // Act
        Result<AccessTokenDoUsuarioDto> resultado =
            await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(resultado.Value);
        Assert.AreEqual(usuarioId, resultado.Value.UsuarioId);
        Assert.AreEqual(token, resultado.Value.Token);
        Assert.AreEqual(
            accessToken.DataExpiracaoEmUtc,
            resultado.Value.DataExpiracaoEmUtc
        );
    }

    [TestMethod]
    public async Task DeveFalhar_QuandoUsuarioNaoForEncontrado()
    {
        // Arrange
        string email = "usuario@email.com";
        string senha = "Senha@123";

        var gerenciadorDeIdentidade =
            new Mock<IGerenciadorDeIdentidade>();

        var emissorDeTokens =
            new Mock<IEmissorDeTokens>();

        gerenciadorDeIdentidade
            .Setup(g => g.ChecarValidadeDeSenhaAsync(
                email,
                senha
            ))
            .ReturnsAsync((UsuarioDto?)null);

        var handler = new AutenticarUsuarioCommandHandler(
            gerenciadorDeIdentidade.Object,
            emissorDeTokens.Object
        );

        var command = new AutenticarUsuarioCommand(
            email,
            senha
        );

        // Act
        Result<AccessTokenDoUsuarioDto> resultado =
            await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsTrue(
            resultado.Errors.Any(e =>
                e.Message == "Email ou senha inválidos."
            )
        );

        emissorDeTokens.Verify(
            e => e.CriarToken(
                It.IsAny<Guid>(),
                It.IsAny<string>()
            ),
            Times.Never
        );
    }


    [TestMethod]
    public async Task DeveFalhar_QuandoSenhaForIncorreta()
    {
        // Arrange
        string email = "usuario@email.com";
        string senha = "SenhaIncorreta@123";

        var gerenciadorDeIdentidade =
            new Mock<IGerenciadorDeIdentidade>();

        var emissorDeTokens =
            new Mock<IEmissorDeTokens>();

        gerenciadorDeIdentidade
            .Setup(g => g.ChecarValidadeDeSenhaAsync(
                email,
                senha
            ))
            .ReturnsAsync((UsuarioDto?)null);

        var handler = new AutenticarUsuarioCommandHandler(
            gerenciadorDeIdentidade.Object,
            emissorDeTokens.Object
        );

        var command = new AutenticarUsuarioCommand(
            email,
            senha
        );

        // Act
        Result<AccessTokenDoUsuarioDto> resultado =
            await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsTrue(
            resultado.Errors.Any(e =>
                e.Message == "Email ou senha inválidos."
            )
        );

        emissorDeTokens.Verify(
            e => e.CriarToken(
                It.IsAny<Guid>(),
                It.IsAny<string>()
            ),
            Times.Never
        );
    }

}