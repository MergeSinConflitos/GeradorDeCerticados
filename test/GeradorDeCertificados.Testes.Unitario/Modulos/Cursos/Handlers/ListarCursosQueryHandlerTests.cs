
using FluentResults;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos.DTOs;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GeradorDeCertificados.Testes.Unitario.Modulos.Cursos.Handlers;

[TestClass]
public sealed class ListarCursosQueryHandlerTests
{
    private readonly Mock<IRepositorioCurso> repositorioCurso;
    private readonly ListarCursosQueryHandler handler;

    public ListarCursosQueryHandlerTests()
    {
        repositorioCurso = new Mock<IRepositorioCurso>();

        handler = new ListarCursosQueryHandler(
            repositorioCurso.Object);
    }

    [TestMethod]
    public async Task DeveListar_TodosOsCursos()
    {
        // Arrange
        Curso curso1 = new(
            Guid.NewGuid(),
            "Curso de C#",
            "Curso completo de C# e .NET",
            40,
            new DateTime(2026, 9, 20));

        Curso curso2 = new(
            Guid.NewGuid(),
            "Curso de React",
            "Fundamentos do React",
            30,
            new DateTime(2026, 10, 10));

        List<Curso> cursos = [curso1, curso2];

        repositorioCurso
            .Setup(x => x.SelecionarTodosAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursos);

        ListarCursosQuery query = new();

        // Act
        Result<List<CursoDto>> resultado =
            await handler.Handle(query);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.HasCount(2, resultado.Value);

        Assert.AreEqual(curso1.Id, resultado.Value[0].Id);
        Assert.AreEqual("Curso de C#", resultado.Value[0].Nome);
        Assert.AreEqual(
            "Curso completo de C# e .NET",
            resultado.Value[0].Descricao);
        Assert.AreEqual(40, resultado.Value[0].CargaHoraria);
        Assert.AreEqual(
            new DateTime(2026, 9, 20),
            resultado.Value[0].DataConclusao);

        Assert.AreEqual(curso2.Id, resultado.Value[1].Id);
        Assert.AreEqual("Curso de React", resultado.Value[1].Nome);
        Assert.AreEqual(
            "Fundamentos do React",
            resultado.Value[1].Descricao);
        Assert.AreEqual(30, resultado.Value[1].CargaHoraria);
        Assert.AreEqual(
            new DateTime(2026, 10, 10),
            resultado.Value[1].DataConclusao);

        repositorioCurso.Verify(
            x => x.SelecionarTodosAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task DeveRetornar_ListaVaziaQuandoNaoExistiremCursos()
    {
        // Arrange
        repositorioCurso
            .Setup(x => x.SelecionarTodosAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        ListarCursosQuery query = new();

        // Act
        Result<List<CursoDto>> resultado =
            await handler.Handle(query);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsEmpty(resultado.Value);

        repositorioCurso.Verify(
            x => x.SelecionarTodosAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
