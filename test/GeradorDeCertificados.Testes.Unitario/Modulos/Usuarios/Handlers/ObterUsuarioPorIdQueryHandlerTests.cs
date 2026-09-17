
using GeradorDeCertificados.Aplicacao.Compartilhado;
using GeradorDeCertificados.Aplicacao.Modulos.Usuarios;
using GeradorDeCertificados.Dominio.Compartilhado.Auth;
using FluentResults;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GeradorDeCertificados.Testes.Unitario.Modulos.Usuarios.Handlers;

[TestClass]
public sealed class ObterUsuarioPorIdQueryHandlerTests
{
    [TestMethod]
    public async Task DeveObter_UsuarioPorId()
    {
        // Arrange
        Guid usuarioId = Guid.CreateVersion7();
        string email = "usuario@email.com";

        var usuario = new UsuarioDto(
            usuarioId,
            email
        );

        var gerenciadorDeIdentidade =
            new Mock<IGerenciadorDeIdentidade>();

        gerenciadorDeIdentidade
            .Setup(g => g.ObterPorIdAsync(usuarioId))
            .ReturnsAsync(usuario);

        var handler = new ObterUsuarioPorIdQueryHandler(
            gerenciadorDeIdentidade.Object
        );

        var query = new ObterUsuarioPorIdQuery(
            usuarioId
        );

        // Act
        Result<UsuarioDto> resultado =
            await handler.Handle(query);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(usuarioId, resultado.Value.Id);
        Assert.AreEqual(email, resultado.Value.Email);
    }

    [TestMethod]
    public async Task DeveFalhar_QuandoUsuarioNaoForEncontrado()
    {
        // Arrange
        Guid usuarioId = Guid.CreateVersion7();

        var gerenciadorDeIdentidade =
            new Mock<IGerenciadorDeIdentidade>();

        gerenciadorDeIdentidade
            .Setup(g => g.ObterPorIdAsync(usuarioId))
            .ReturnsAsync((UsuarioDto?)null);

        var handler = new ObterUsuarioPorIdQueryHandler(
            gerenciadorDeIdentidade.Object
        );

        var query = new ObterUsuarioPorIdQuery(
            usuarioId
        );

        // Act
        Result<UsuarioDto> resultado =
            await handler.Handle(query);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "Usuário não encontrado.",
            resultado.Errors.First().Message
        );
    }
}
