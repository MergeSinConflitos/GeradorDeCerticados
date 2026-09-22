using GeradorDeCertificados.Aplicacao.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using MassTransit;
using Moq;

namespace GeradorDeCertificados.Testes.Aplicacao.Modulos.Certificados;

[TestClass]
public sealed class SolicitarCertificadosCommandHandlerTests
{
    private Mock<IRepositorioCurso> repositorioCurso = null!;
    private Mock<IRepositorioSolicitacaoCertificado> repositorioSolicitacaoCertificado = null!;
    private SolicitarCertificadosCommandHandler handler = null!;
    private Mock<IPublishEndpoint> enpoint = null!;

    [TestInitialize]
    public void Setup()
    {
        repositorioCurso = new Mock<IRepositorioCurso>();

        repositorioSolicitacaoCertificado =
            new Mock<IRepositorioSolicitacaoCertificado>();

        enpoint = new Mock<IPublishEndpoint>();

        handler = new SolicitarCertificadosCommandHandler(
            repositorioCurso.Object,
            repositorioSolicitacaoCertificado.Object,
            enpoint.Object);
    }

    [TestMethod]
    public async Task DeveSolicitarCertificados()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();

        Curso curso = new(
            cursoId,
            "Curso de C#",
            "Curso completo de C# e .NET",
            40,
            new DateTime(2026, 9, 20));

        repositorioCurso
            .Setup(x => x.SelecionarPorIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(curso);

        repositorioSolicitacaoCertificado
            .Setup(x => x.ExisteProcessamentoEmAndamentoAsync(
                cursoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        repositorioSolicitacaoCertificado
            .Setup(x => x.CadastrarAsync(
                It.IsAny<SolicitacaoCertificado>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        SolicitarCertificadosCommand request =
            new(
                cursoId,
                [
                    "Aluno 1",
                    "Aluno 2"
                ]);

        // Act
        var resultado = await handler.Handle(
            request,
            CancellationToken.None);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(cursoId, resultado.Value.CursoId);
        Assert.HasCount(2, resultado.Value.Certificados);

        Assert.AreEqual(
            "Aluno 1",
            resultado.Value.Certificados[0].NomeAluno);

        Assert.AreEqual(
            "Aluno 2",
            resultado.Value.Certificados[1].NomeAluno);

        repositorioCurso.Verify(
            x => x.SelecionarPorIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repositorioSolicitacaoCertificado.Verify(
            x => x.ExisteProcessamentoEmAndamentoAsync(
                cursoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repositorioSolicitacaoCertificado.Verify(
            x => x.CadastrarAsync(
                It.IsAny<SolicitacaoCertificado>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task DeveRetornarErro_QuandoCursoNaoForEncontrado()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();

        repositorioCurso
            .Setup(x => x.SelecionarPorIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Curso?)null);

        SolicitarCertificadosCommand request =
            new(
                cursoId,
                ["Aluno 1"]);

        // Act
        var resultado = await handler.Handle(
            request,
            CancellationToken.None);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        repositorioCurso.Verify(
            x => x.SelecionarPorIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repositorioSolicitacaoCertificado.Verify(
            x => x.ExisteProcessamentoEmAndamentoAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        repositorioSolicitacaoCertificado.Verify(
            x => x.CadastrarAsync(
                It.IsAny<SolicitacaoCertificado>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    public async Task DeveRetornarErro_QuandoHouverProcessamentoEmAndamento()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();

        Curso curso = new(
            cursoId,
            "Curso de C#",
            "Curso completo de C# e .NET",
            40,
            new DateTime(2026, 9, 20));

        repositorioCurso
            .Setup(x => x.SelecionarPorIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(curso);

        repositorioSolicitacaoCertificado
            .Setup(x => x.ExisteProcessamentoEmAndamentoAsync(
                cursoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        SolicitarCertificadosCommand request =
            new(
                cursoId,
                ["Aluno 1"]);

        // Act
        var resultado = await handler.Handle(
            request,
            CancellationToken.None);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        repositorioSolicitacaoCertificado.Verify(
            x => x.ExisteProcessamentoEmAndamentoAsync(
                cursoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repositorioSolicitacaoCertificado.Verify(
            x => x.CadastrarAsync(
                It.IsAny<SolicitacaoCertificado>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}