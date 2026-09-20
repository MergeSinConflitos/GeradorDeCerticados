using FizzWare.NBuilder;
using GeradorDeCertificados.Dominio.Modulos.Certificados;

namespace GeradorDeCertificados.Testes.Integracao.Modulos.Certificados;

[TestClass]
public sealed class RepositorioCertificadoEmOrmTests
    : RepositorioBaseEmOrmTests
{
    [TestMethod]
    public async Task DeveCadastrar_Certificado()
    {
        Certificado certificado = Builder<Certificado>
            .CreateNew()
            .With(x => x.Id = Guid.NewGuid())
            .With(x => x.CursoId = Guid.NewGuid())
            .With(x => x.NomeAluno = "João da Silva")
            .Build();

        await repositorioCertificado.CadastrarAsync(certificado);

        Certificado? certificadoSalvo =
            await repositorioCertificado.SelecionarPorIdAsync(certificado.Id);

        Assert.IsNotNull(certificadoSalvo);
        Assert.AreEqual(certificado.Id, certificadoSalvo.Id);
        Assert.AreEqual(certificado.CursoId, certificadoSalvo.CursoId);
        Assert.AreEqual(certificado.NomeAluno, certificadoSalvo.NomeAluno);
    }

    [TestMethod]
    public async Task DeveObter_CertificadoPorId()
    {
        Certificado certificado = Builder<Certificado>
            .CreateNew()
            .With(x => x.Id = Guid.NewGuid())
            .With(x => x.CursoId = Guid.NewGuid())
            .With(x => x.NomeAluno = "Maria da Silva")
            .Build();

        await repositorioCertificado.CadastrarAsync(certificado);

        Certificado? resultado =
            await repositorioCertificado.SelecionarPorIdAsync(certificado.Id);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(certificado.Id, resultado.Id);
        Assert.AreEqual("Maria da Silva", resultado.NomeAluno);
    }

    [TestMethod]
    public async Task DeveListar_Certificados()
    {
        List<Certificado> certificados = Builder<Certificado>
            .CreateListOfSize(3)
            .All()
            .With(x => x.Id = Guid.NewGuid())
            .With(x => x.CursoId = Guid.NewGuid())
            .With(x => x.NomeAluno = "Aluno")
            .Build()
            .ToList();

        foreach (Certificado certificado in certificados)
        {
            await repositorioCertificado.CadastrarAsync(certificado);
        }

        IReadOnlyList<Certificado> resultado =
            await repositorioCertificado.SelecionarTodosAsync();

        Assert.AreEqual(3, resultado.Count);
    }

    [TestMethod]
    public async Task DeveAtualizar_Certificado()
    {
        Certificado certificado = Builder<Certificado>
            .CreateNew()
            .With(x => x.Id = Guid.NewGuid())
            .With(x => x.CursoId = Guid.NewGuid())
            .With(x => x.NomeAluno = "João da Silva")
            .Build();

        await repositorioCertificado.CadastrarAsync(certificado);

        Certificado certificadoAtualizado = Builder<Certificado>
            .CreateNew()
            .With(x => x.Id = certificado.Id)
            .With(x => x.CursoId = certificado.CursoId)
            .With(x => x.NomeAluno = "João da Silva Atualizado")
            .Build();

        await repositorioCertificado.EditarAsync(certificado.Id, certificadoAtualizado);

        Certificado? resultado =
            await repositorioCertificado.SelecionarPorIdAsync(certificado.Id);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(
            "João da Silva Atualizado",
            resultado.NomeAluno);
    }

    [TestMethod]
    public async Task DeveExcluir_Certificado()
    {
        Certificado certificado = Builder<Certificado>
            .CreateNew()
            .With(x => x.Id = Guid.NewGuid())
            .With(x => x.CursoId = Guid.NewGuid())
            .With(x => x.NomeAluno = "Pedro da Silva")
            .Build();

        await repositorioCertificado.CadastrarAsync(certificado);

        await repositorioCertificado.ExcluirAsync(certificado.Id);

        Certificado? resultado =
            await repositorioCertificado.SelecionarPorIdAsync(certificado.Id);

        Assert.IsNull(resultado);
    }
}