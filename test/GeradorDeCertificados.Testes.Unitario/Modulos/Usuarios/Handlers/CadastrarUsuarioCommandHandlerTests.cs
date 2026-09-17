using FluentResults;
using GeradorDeCertificados.Aplicacao.Modulos.Usuarios;
using GeradorDeCertificados.Dominio.Compartilhado.Auth;
using Moq;

namespace GeradorDeCertificados.Testes.Unitario.Modulos.Usuarios.Handlers;

[TestClass]
public sealed class CadastrarUsuarioCommandHandlerTests
{
    [TestMethod]
    public async Task DeveCadastrar_QuandoDadosForemValidos()
    {
        // Arrange
        string email = "usuario@email.com";
        string senha = "Senha@123";

        Guid usuarioId = Guid.CreateVersion7();

        var usuario = new UsuarioDto(
            usuarioId,
            email
        );

        var gerenciadorDeIdentidade =
            new Mock<IGerenciadorDeIdentidade>();

        gerenciadorDeIdentidade
            .Setup(g => g.CadastrarAsync(
                It.IsAny<Guid>(),
                email,
                senha
            ))
            .ReturnsAsync(usuario);

        var handler = new CadastrarUsuarioCommandHandler(
            gerenciadorDeIdentidade.Object
        );

        var command = new CadastrarUsuarioCommand(
            email,
            senha
        );

        // Act
        Result<Guid> resultado =
            await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(usuarioId, resultado.Value);

        gerenciadorDeIdentidade.Verify(
            g => g.CadastrarAsync(
                It.IsAny<Guid>(),
                email,
                senha
            ),
            Times.Once
        );
    }

    [TestMethod]
    public async Task DeveFalhar_QuandoEmailJaEstiverCadastrado()
    {
        // Arrange
        string email = "usuario@email.com";
        string senha = "Senha@123";

        string mensagem =
            "Já existe um usuário cadastrado com este email.";

        var gerenciadorDeIdentidade =
            new Mock<IGerenciadorDeIdentidade>();

        gerenciadorDeIdentidade
            .Setup(g => g.CadastrarAsync(
                It.IsAny<Guid>(),
                email,
                senha
            ))
            .ThrowsAsync(
                new ConflitoDeIdentidadeException(mensagem)
            );

        var handler = new CadastrarUsuarioCommandHandler(
            gerenciadorDeIdentidade.Object
        );

        var command = new CadastrarUsuarioCommand(
            email,
            senha
        );

        // Act
        Result<Guid> resultado =
            await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            mensagem,
            resultado.Errors.First().Message
        );
    }

    [TestMethod]
    public async Task DeveFalhar_QuandoEmailForInvalido()
    {
        // Arrange
        string email = "email-invalido";
        string senha = "Senha@123";

        string mensagem = "Email inválido.";

        var gerenciadorDeIdentidade =
            new Mock<IGerenciadorDeIdentidade>();

        gerenciadorDeIdentidade
            .Setup(g => g.CadastrarAsync(
                It.IsAny<Guid>(),
                email,
                senha
            ))
            .ThrowsAsync(
                new ValidacaoDeIdentidadeException(
                    "Email",
                    mensagem
                )
            );

        var handler = new CadastrarUsuarioCommandHandler(
            gerenciadorDeIdentidade.Object
        );

        var command = new CadastrarUsuarioCommand(
            email,
            senha
        );

        // Act
        Result<Guid> resultado =
            await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "Email: " + mensagem,
            resultado.Errors.First().Message
        );
    }

    [TestMethod]
    public async Task DeveFalhar_QuandoSenhaForMenorQueOitoCaracteres()
    {
        // Arrange
        string email = "usuario@email.com";
        string senha = "Sen@123";

        string mensagem =
            "A senha deve possuir no mínimo 8 caracteres.";

        var gerenciadorDeIdentidade =
            new Mock<IGerenciadorDeIdentidade>();

        gerenciadorDeIdentidade
            .Setup(g => g.CadastrarAsync(
                It.IsAny<Guid>(),
                email,
                senha
            ))
            .ThrowsAsync(
                new ValidacaoDeIdentidadeException(
                    "Senha",
                    mensagem
                )
            );

        var handler = new CadastrarUsuarioCommandHandler(
            gerenciadorDeIdentidade.Object
        );

        var command = new CadastrarUsuarioCommand(
            email,
            senha
        );

        // Act
        Result<Guid> resultado =
            await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "Senha: " + mensagem,
            resultado.Errors.First().Message
        );
    }

    [TestMethod]
    public async Task DeveFalhar_QuandoSenhaNaoPossuirNumero()
    {
        // Arrange
        string email = "usuario@email.com";
        string senha = "Senha@abc";

        string mensagem =
            "A senha deve possuir pelo menos um número.";

        var gerenciadorDeIdentidade =
            new Mock<IGerenciadorDeIdentidade>();

        gerenciadorDeIdentidade
            .Setup(g => g.CadastrarAsync(
                It.IsAny<Guid>(),
                email,
                senha
            ))
            .ThrowsAsync(
                new ValidacaoDeIdentidadeException(
                    "Senha",
                    mensagem
                )
            );

        var handler = new CadastrarUsuarioCommandHandler(
            gerenciadorDeIdentidade.Object
        );

        var command = new CadastrarUsuarioCommand(
            email,
            senha
        );

        // Act
        Result<Guid> resultado =
            await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "Senha: " + mensagem,
            resultado.Errors.First().Message
        );
    }

    [TestMethod]
    public async Task DeveFalhar_QuandoSenhaNaoPossuirCaractereNaoAlfanumerico()
    {
        // Arrange
        string email = "usuario@email.com";
        string senha = "Senha123";

        string mensagem =
            "A senha deve possuir pelo menos um caractere não alfanumérico.";

        var gerenciadorDeIdentidade =
            new Mock<IGerenciadorDeIdentidade>();

        gerenciadorDeIdentidade
            .Setup(g => g.CadastrarAsync(
                It.IsAny<Guid>(),
                email,
                senha
            ))
            .ThrowsAsync(
                new ValidacaoDeIdentidadeException(
                    "Senha",
                    mensagem
                )
            );

        var handler = new CadastrarUsuarioCommandHandler(
            gerenciadorDeIdentidade.Object
        );

        var command = new CadastrarUsuarioCommand(
            email,
            senha
        );

        // Act
        Result<Guid> resultado =
            await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "Senha: " + mensagem,
            resultado.Errors.First().Message
        );
    }
}