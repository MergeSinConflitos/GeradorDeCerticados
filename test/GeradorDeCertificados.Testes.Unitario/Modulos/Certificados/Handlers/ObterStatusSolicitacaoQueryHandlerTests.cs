using GeradorDeCertificados.Aplicacao.Compartilhado;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using Moq;

namespace GeradorDeCertificados.Testes.Aplicacao.Modulos.Certificados;

[TestClass]
public sealed class ObterStatusSolicitacaoQueryHandlerTests
{
    private Mock<IRepositorioSolicitacaoCertificado> repositorioSolicitacaoCertificado = null!;
    private ObterStatusSolicitacaoQueryHandler handler = null!;

    [TestInitialize]
    public void Setup()
    {
        repositorioSolicitacaoCertificado =
            new Mock<IRepositorioSolicitacaoCertificado>();

        handler = new ObterStatusSolicitacaoQueryHandler(
            repositorioSolicitacaoCertificado.Object);
    }

    [TestMethod]
    public async Task DeveObterStatusDaSolicitacao()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();
        Guid solicitacaoId = Guid.CreateVersion7();

        SolicitacaoCertificado solicitacao =
            new(
                solicitacaoId,
                cursoId,
                []);

        solicitacao.IniciarProcessamento();

        repositorioSolicitacaoCertificado
            .Setup(x => x.SelecionarPorCursoIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(solicitacao);

        ObterStatusSolicitacaoQuery request =
            new(cursoId);

        // Act
        var resultado = await handler.Handle(
            request,
            CancellationToken.None);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(
            StatusProcessamento.GerandoCertificados,
            resultado.Value);

        repositorioSolicitacaoCertificado.Verify(
            x => x.SelecionarPorCursoIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task DeveRetornarErro_QuandoSolicitacaoNaoForEncontrada()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();

        repositorioSolicitacaoCertificado
            .Setup(x => x.SelecionarPorCursoIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((SolicitacaoCertificado?)null);

        ObterStatusSolicitacaoQuery request =
            new(cursoId);

        // Act
        var resultado = await handler.Handle(
            request,
            CancellationToken.None);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "NaoEncontrado",
            resultado.Errors[0].Metadata[nameof(TipoErro)].ToString());

        repositorioSolicitacaoCertificado.Verify(
            x => x.SelecionarPorCursoIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}