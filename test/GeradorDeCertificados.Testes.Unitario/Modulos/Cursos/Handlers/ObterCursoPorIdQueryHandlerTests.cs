using FluentResults;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos.DTOs;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GeradorDeCertificados.Testes.Unitario.Modulos.Cursos.Handlers;

[TestClass]
public sealed class ObterCursoPorIdQueryHandlerTests
{
    private readonly Mock<IRepositorioCurso> repositorioCurso;
    private readonly ObterCursoPorIdQueryHandler handler;

    public ObterCursoPorIdQueryHandlerTests()
    {
        repositorioCurso = new Mock<IRepositorioCurso>();

        handler = new ObterCursoPorIdQueryHandler(
            repositorioCurso.Object);
    }

    [TestMethod]
    public async Task DeveObter_CursoPorId()
    {
        // Arrange
        Guid cursoId = Guid.NewGuid();

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

        ObterCursoPorIdQuery query = new(cursoId);

        // Act
        Result<CursoDto> resultado =
            await handler.Handle(query);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(cursoId, resultado.Value.Id);
        Assert.AreEqual("Curso de C#", resultado.Value.Nome);
        Assert.AreEqual(
            "Curso completo de C# e .NET",
            resultado.Value.Descricao);
        Assert.AreEqual(40, resultado.Value.CargaHoraria);
        Assert.AreEqual(
            new DateTime(2026, 9, 20),
            resultado.Value.DataConclusao);

        repositorioCurso.Verify(
            x => x.SelecionarPorIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoCursoNaoForEncontrado()
    {
        // Arrange
        Guid cursoId = Guid.NewGuid();

        repositorioCurso
            .Setup(x => x.SelecionarPorIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Curso?)null);

        ObterCursoPorIdQuery query = new(cursoId);

        // Act
        Result<CursoDto> resultado =
            await handler.Handle(query);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsFalse(resultado.IsSuccess);

        repositorioCurso.Verify(
            x => x.SelecionarPorIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}