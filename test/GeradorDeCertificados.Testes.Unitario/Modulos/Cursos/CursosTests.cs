using GeradorDeCertificados.Dominio.Compartilhado;
using GeradorDeCertificados.Dominio.Modulos.Cursos;

namespace GeradorDeCertificados.Testes.Unitario.Modulos.Cursos;

[TestClass]
public sealed class CursoTests
{
    [TestMethod]
    public void DeveRetornar_ErroQuandoNomeNaoForInformado()
    {
        // Arrange
        Curso curso = new(
            Guid.NewGuid(),
            string.Empty,
            "Descrição do curso",
            40,
            new DateTime(2026, 9, 20));

        // Act
        IReadOnlyList<ErroValidacao> erros =
            curso.Validar();

        // Assert
        Assert.IsTrue(erros.Any());
        Assert.AreEqual(
            nameof(Curso.Nome),
            erros[0].Campo);
    }

    [TestMethod]
    public void DeveRetornar_ErroQuandoNomePossuirMaisDe200Caracteres()
    {
        // Arrange
        string nome = new('A', 201);

        Curso curso = new(
            Guid.NewGuid(),
            nome,
            "Descrição do curso",
            40,
            new DateTime(2026, 9, 20));

        // Act
        IReadOnlyList<ErroValidacao> erros =
            curso.Validar();

        // Assert
        Assert.IsTrue(erros.Any());
        Assert.AreEqual(
            nameof(Curso.Nome),
            erros[0].Campo);
    }

    [TestMethod]
    public void DeveRetornar_ErroQuandoDescricaoPossuirMaisDe500Caracteres()
    {
        // Arrange
        string descricao = new('A', 501);

        Curso curso = new(
            Guid.NewGuid(),
            "Curso de C#",
            descricao,
            40,
            new DateTime(2026, 9, 20));

        // Act
        IReadOnlyList<ErroValidacao> erros =
            curso.Validar();

        // Assert
        Assert.IsTrue(erros.Any());
        Assert.AreEqual(
            nameof(Curso.Descricao),
            erros[0].Campo);
    }

    [TestMethod]
    public void DeveRetornar_ErroQuandoCargaHorariaNaoForInformada()
    {
        // Arrange
        Curso curso = new(
            Guid.NewGuid(),
            "Curso de C#",
            "Descrição do curso",
            0,
            new DateTime(2026, 9, 20));

        // Act
        IReadOnlyList<ErroValidacao> erros =
            curso.Validar();

        // Assert
        Assert.IsTrue(erros.Any());
        Assert.AreEqual(
            nameof(Curso.CargaHoraria),
            erros[0].Campo);
    }

    [TestMethod]
    public void DeveRetornar_ErroQuandoCargaHorariaForNegativa()
    {
        // Arrange
        Curso curso = new(
            Guid.NewGuid(),
            "Curso de C#",
            "Descrição do curso",
            -1,
            new DateTime(2026, 9, 20));

        // Act
        IReadOnlyList<ErroValidacao> erros =
            curso.Validar();

        // Assert
        Assert.IsTrue(erros.Any());
        Assert.AreEqual(
            nameof(Curso.CargaHoraria),
            erros[0].Campo);
    }

    [TestMethod]
    public void DeveRetornar_ErroQuandoDataConclusaoNaoForInformada()
    {
        // Arrange
        Curso curso = new(
            Guid.NewGuid(),
            "Curso de C#",
            "Descrição do curso",
            40,
            DateTime.MinValue);

        // Act
        IReadOnlyList<ErroValidacao> erros =
            curso.Validar();

        // Assert
        Assert.IsTrue(erros.Any());
        Assert.AreEqual(
            nameof(Curso.DataConclusao),
            erros[0].Campo);
    }

    [TestMethod]
    public void DeveCriar_CursoComDadosInformados()
    {
        // Arrange
        Guid cursoId = Guid.NewGuid();
        string nome = "Curso de C#";
        string descricao = "Curso completo de C# e .NET";
        int cargaHoraria = 40;
        DateTime dataConclusao =
            new(2026, 9, 20);

        // Act
        Curso curso = new(
            cursoId,
            nome,
            descricao,
            cargaHoraria,
            dataConclusao);

        // Assert
        Assert.AreEqual(cursoId, curso.Id);
        Assert.AreEqual(nome, curso.Nome);
        Assert.AreEqual(descricao, curso.Descricao);
        Assert.AreEqual(cargaHoraria, curso.CargaHoraria);
        Assert.AreEqual(dataConclusao, curso.DataConclusao);
    }

    [TestMethod]
    public void DeveCriar_CursoComDescricaoNaoInformada()
    {
        // Arrange
        Curso curso = new(
            Guid.NewGuid(),
            "Curso de C#",
            null,
            40,
            new DateTime(2026, 9, 20));

        // Act
        IReadOnlyList<ErroValidacao> erros =
            curso.Validar();

        // Assert
        Assert.IsFalse(erros.Any());
        Assert.IsNull(curso.Descricao);
    }

    [TestMethod]
    public void DeveAtualizar_DadosDoCurso()
    {
        // Arrange
        Curso curso = new(
            Guid.NewGuid(),
            "Curso de C#",
            "Descrição antiga",
            40,
            new DateTime(2026, 9, 20));

        Curso cursoAtualizado = new(
            Guid.NewGuid(),
            "Curso de .NET",
            "Descrição nova",
            60,
            new DateTime(2026, 10, 10));

        // Act
        curso.Atualizar(cursoAtualizado);

        // Assert
        Assert.AreEqual(
            cursoAtualizado.Nome,
            curso.Nome);

        Assert.AreEqual(
            cursoAtualizado.Descricao,
            curso.Descricao);

        Assert.AreEqual(
            cursoAtualizado.CargaHoraria,
            curso.CargaHoraria);

        Assert.AreEqual(
            cursoAtualizado.DataConclusao,
            curso.DataConclusao);
    }
}