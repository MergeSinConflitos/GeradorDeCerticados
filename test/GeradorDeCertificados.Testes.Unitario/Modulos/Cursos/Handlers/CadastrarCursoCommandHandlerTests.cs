using FluentResults;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos;
using GeradorDeCertificados.Dominio.Compartilhado;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GeradorDeCertificados.Testes.Unitario.Modulos.Cursos.Handlers;

[TestClass]
public sealed class CadastrarCursoCommandHandlerTests
{
    private readonly Mock<IRepositorioCurso> repositorioCurso;
    private readonly CadastrarCursoCommandHandler handler;

    public CadastrarCursoCommandHandlerTests()
    {
        repositorioCurso = new Mock<IRepositorioCurso>();

        handler = new CadastrarCursoCommandHandler(
            repositorioCurso.Object);
    }

    [TestMethod]
    public async Task DeveCadastrar_Curso()
    {
        // Arrange
        CadastrarCursoCommand command = new(
            "Curso de C#",
            "Curso completo de C# e .NET",
            40,
            new DateTime(2026, 9, 20));

        // Act
        Result<Guid> resultado =
            await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreNotEqual(Guid.Empty, resultado.Value);

        repositorioCurso.Verify(
            x => x.CadastrarAsync(
                It.Is<Curso>(curso =>
                    curso.Nome == command.Nome &&
                    curso.Descricao == command.Descricao &&
                    curso.CargaHoraria == command.CargaHoraria &&
                    curso.DataConclusao == command.DataConclusao),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoNomeNaoForInformado()
    {
        // Arrange
        CadastrarCursoCommand command = new(
            string.Empty,
            "Descrição do curso",
            40,
            new DateTime(2026, 9, 20));

        // Act
        Result<Guid> resultado =
            await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        Assert.AreEqual(
            nameof(Curso.Nome),
            resultado.Errors[0].Metadata["Campo"]);

        repositorioCurso.Verify(
            x => x.CadastrarAsync(
                It.IsAny<Curso>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoNomePossuirMaisDe200Caracteres()
    {
        // Arrange
        string nome = new('A', 201);

        CadastrarCursoCommand command = new(
            nome,
            "Descrição do curso",
            40,
            new DateTime(2026, 9, 20));

        // Act
        Result<Guid> resultado =
            await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        Assert.AreEqual(
            nameof(Curso.Nome),
            resultado.Errors[0].Metadata["Campo"]);

        repositorioCurso.Verify(
            x => x.CadastrarAsync(
                It.IsAny<Curso>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoDescricaoPossuirMaisDe500Caracteres()
    {
        // Arrange
        string descricao = new('A', 501);

        CadastrarCursoCommand command = new(
            "Curso de C#",
            descricao,
            40,
            new DateTime(2026, 9, 20));

        // Act
        Result<Guid> resultado =
            await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        Assert.AreEqual(
            nameof(Curso.Descricao),
            resultado.Errors[0].Metadata["Campo"]);

        repositorioCurso.Verify(
            x => x.CadastrarAsync(
                It.IsAny<Curso>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoCargaHorariaForInvalida()
    {
        // Arrange
        CadastrarCursoCommand command = new(
            "Curso de C#",
            "Descrição do curso",
            0,
            new DateTime(2026, 9, 20));

        // Act
        Result<Guid> resultado =
            await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        Assert.AreEqual(
            nameof(Curso.CargaHoraria),
            resultado.Errors[0].Metadata["Campo"]);

        repositorioCurso.Verify(
            x => x.CadastrarAsync(
                It.IsAny<Curso>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoDataConclusaoNaoForInformada()
    {
        // Arrange
        CadastrarCursoCommand command = new(
            "Curso de C#",
            "Descrição do curso",
            40,
            DateTime.MinValue);

        // Act
        Result<Guid> resultado =
            await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        Assert.AreEqual(
            nameof(Curso.DataConclusao),
            resultado.Errors[0].Metadata["Campo"]);

        repositorioCurso.Verify(
            x => x.CadastrarAsync(
                It.IsAny<Curso>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    public async Task DeveCadastrar_CursoSemDescricao()
    {
        // Arrange
        CadastrarCursoCommand command = new(
            "Curso de C#",
            null,
            40,
            new DateTime(2026, 9, 20));

        // Act
        Result<Guid> resultado =
            await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);

        repositorioCurso.Verify(
            x => x.CadastrarAsync(
                It.Is<Curso>(curso =>
                    curso.Nome == command.Nome &&
                    curso.Descricao == null &&
                    curso.CargaHoraria == command.CargaHoraria &&
                    curso.DataConclusao == command.DataConclusao),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoOcorrerConflitoDePersistencia()
    {
        // Arrange
        CadastrarCursoCommand command = new(
            "Curso de C#",
            "Curso completo de C# e .NET",
            40,
            new DateTime(2026, 9, 20));

        repositorioCurso
            .Setup(x => x.CadastrarAsync(
                It.IsAny<Curso>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ConflitoDePersistenciaException("Erro", new Exception()));

        // Act
        Result<Guid> resultado =
            await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        repositorioCurso.Verify(
            x => x.CadastrarAsync(
                It.IsAny<Curso>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}