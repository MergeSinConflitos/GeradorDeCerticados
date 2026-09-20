using GeradorDeCertificados.Aplicacao.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using Moq;

namespace GeradorDeCertificados.Testes.Aplicacao.Modulos.Certificados;

[TestClass]
public sealed class ListarCertificadosQueryHandlerTests
{
    private Mock<IRepositorioCertificado> repositorioCertificado = null!;
    private ListarCertificadosQueryHandler handler = null!;

    [TestInitialize]
    public void Setup()
    {
        repositorioCertificado = new Mock<IRepositorioCertificado>();

        handler = new ListarCertificadosQueryHandler(
            repositorioCertificado.Object);
    }

    [TestMethod]
    public async Task DeveListarCertificados()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();
        Guid solicitacaoId = Guid.CreateVersion7();

        List<Certificado> certificados =
        [
            new Certificado(
                Guid.CreateVersion7(),
                cursoId,
                solicitacaoId,
                "Aluno 1"),

            new Certificado(
                Guid.CreateVersion7(),
                cursoId,
                solicitacaoId,
                "Aluno 2")
        ];

        repositorioCertificado
            .Setup(x => x.ListarPorCursoIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(certificados);

        ListarCertificadosQuery request =
            new(cursoId);

        // Act
        var resultado = await handler.Handle(
            request,
            CancellationToken.None);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.HasCount(2, resultado.Value);

        Assert.AreEqual(
            certificados[0].Id,
            resultado.Value[0].Id);

        Assert.AreEqual(
            certificados[0].NomeAluno,
            resultado.Value[0].NomeAluno);

        Assert.AreEqual(
            certificados[1].Id,
            resultado.Value[1].Id);

        Assert.AreEqual(
            certificados[1].NomeAluno,
            resultado.Value[1].NomeAluno);

        repositorioCertificado.Verify(
            x => x.ListarPorCursoIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task DeveRetornarListaVazia_QuandoNaoExistiremCertificados()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();

        repositorioCertificado
            .Setup(x => x.ListarPorCursoIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        ListarCertificadosQuery request =
            new(cursoId);

        // Act
        var resultado = await handler.Handle(
            request,
            CancellationToken.None);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsEmpty(resultado.Value);

        repositorioCertificado.Verify(
            x => x.ListarPorCursoIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}