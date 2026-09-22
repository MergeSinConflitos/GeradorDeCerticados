using FizzWare.NBuilder;
using GeradorDeCertificados.Dominio.Modulos.Cursos;

namespace GeradorDeCertificados.Testes.Integracao.Modulos.Cursos;

[TestClass]
public sealed class RepositorioCursoEmOrmTests
    : RepositorioBaseEmOrmTests
{
    [TestMethod]
    public async Task DeveCadastrar_Curso()
    {
        Curso curso = Builder<Curso>
            .CreateNew()
            .With(x => x.Id = Guid.NewGuid())
            .With(x => x.Nome = "Curso de C#")
            .With(x => x.Descricao = "Curso completo de C# e .NET")
            .With(x => x.CargaHoraria = 40)
            .With(x => x.DataConclusao = new DateTime(2026, 9, 20))
            .Build();

        await repositorioCurso.CadastrarAsync(curso);

        Curso? cursoSalvo =
            await repositorioCurso.SelecionarPorIdAsync(curso.Id);

        Assert.IsNotNull(cursoSalvo);
        Assert.AreEqual(curso.Id, cursoSalvo.Id);
        Assert.AreEqual(curso.Nome, cursoSalvo.Nome);
        Assert.AreEqual(curso.Descricao, cursoSalvo.Descricao);
        Assert.AreEqual(curso.CargaHoraria, cursoSalvo.CargaHoraria);
        Assert.AreEqual(curso.DataConclusao, cursoSalvo.DataConclusao);
    }

    [TestMethod]
    public async Task DeveObter_CursoPorId()
    {
        Curso curso = Builder<Curso>
            .CreateNew()
            .With(x => x.Id = Guid.NewGuid())
            .With(x => x.Nome = "Curso de .NET")
            .With(x => x.Descricao = "Curso completo de .NET")
            .With(x => x.CargaHoraria = 60)
            .With(x => x.DataConclusao = new DateTime(2026, 9, 20))
            .Build();

        await repositorioCurso.CadastrarAsync(curso);

        Curso? resultado =
            await repositorioCurso.SelecionarPorIdAsync(curso.Id);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(curso.Id, resultado.Id);
        Assert.AreEqual("Curso de .NET", resultado.Nome);
    }

    [TestMethod]
    public async Task DeveListar_Cursos()
    {
        List<Curso> cursos = Builder<Curso>
            .CreateListOfSize(3)
            .All()
            .With(x => x.Id = Guid.NewGuid())
            .With(x => x.Nome = "Curso")
            .With(x => x.Descricao = "Descrição do curso")
            .With(x => x.CargaHoraria = 40)
            .With(x => x.DataConclusao = new DateTime(2026, 9, 20))
            .Build()
            .ToList();

        foreach (Curso curso in cursos)
        {
            await repositorioCurso.CadastrarAsync(curso);
        }

        IReadOnlyList<Curso> resultado =
            await repositorioCurso.SelecionarTodosAsync();

        Assert.AreEqual(3, resultado.Count);
    }

    [TestMethod]
    public async Task DeveAtualizar_Curso()
    {
        Curso curso = Builder<Curso>
            .CreateNew()
            .With(x => x.Id = Guid.NewGuid())
            .With(x => x.Nome = "Curso de C#")
            .With(x => x.Descricao = "Descrição antiga")
            .With(x => x.CargaHoraria = 40)
            .With(x => x.DataConclusao = new DateTime(2026, 9, 20))
            .Build();

        await repositorioCurso.CadastrarAsync(curso);

        Curso cursoAtualizado = Builder<Curso>
            .CreateNew()
            .With(x => x.Id = curso.Id)
            .With(x => x.Nome = "Curso de .NET")
            .With(x => x.Descricao = "Descrição nova")
            .With(x => x.CargaHoraria = 60)
            .With(x => x.DataConclusao = new DateTime(2026, 10, 10))
            .Build();

        await repositorioCurso.EditarAsync(
            curso.Id,
            cursoAtualizado);

        Curso? resultado =
            await repositorioCurso.SelecionarPorIdAsync(curso.Id);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(
            "Curso de .NET",
            resultado.Nome);
        Assert.AreEqual(
            "Descrição nova",
            resultado.Descricao);
        Assert.AreEqual(
            60,
            resultado.CargaHoraria);
        Assert.AreEqual(
            new DateTime(2026, 10, 10),
            resultado.DataConclusao);
    }

    [TestMethod]
    public async Task DeveExcluir_Curso()
    {
        Curso curso = Builder<Curso>
            .CreateNew()
            .With(x => x.Id = Guid.NewGuid())
            .With(x => x.Nome = "Curso de C#")
            .With(x => x.Descricao = "Descrição do curso")
            .With(x => x.CargaHoraria = 40)
            .With(x => x.DataConclusao = new DateTime(2026, 9, 20))
            .Build();

        await repositorioCurso.CadastrarAsync(curso);

        await repositorioCurso.ExcluirAsync(curso.Id);

        Curso? resultado =
            await repositorioCurso.SelecionarPorIdAsync(curso.Id);

        Assert.IsNull(resultado);
    }
}