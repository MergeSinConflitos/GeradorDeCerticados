using FizzWare.NBuilder;
using GeradorDeCertificados.Dominio.Modulos.Certificados;

namespace GeradorDeCertificados.Testes.Integracao.Modulos.Certificados;

[TestClass]
public sealed class RepositorioSolicitacaoCertificadoEmOrmTests
    : RepositorioBaseEmOrmTests
{
    [TestMethod]
    public async Task DeveCadastrar_SolicitacaoCertificado()
    {
        SolicitacaoCertificado solicitacao = Builder<SolicitacaoCertificado>
            .CreateNew()
            .With(x => x.Id = Guid.NewGuid())
            .With(x => x.CursoId = Guid.NewGuid())
            .Build();

        await repositorioSolicitacaoCertificado
            .CadastrarAsync(solicitacao);

        SolicitacaoCertificado? resultado =
            await repositorioSolicitacaoCertificado
                .SelecionarPorIdAsync(solicitacao.Id);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(solicitacao.Id, resultado.Id);
        Assert.AreEqual(solicitacao.CursoId, resultado.CursoId);
    }

    [TestMethod]
    public async Task DeveObter_SolicitacaoCertificadoPorId()
    {
        SolicitacaoCertificado solicitacao = Builder<SolicitacaoCertificado>
            .CreateNew()
            .With(x => x.Id = Guid.NewGuid())
            .With(x => x.CursoId = Guid.NewGuid())
            .Build();

        await repositorioSolicitacaoCertificado
            .CadastrarAsync(solicitacao);

        SolicitacaoCertificado? resultado =
            await repositorioSolicitacaoCertificado
                .SelecionarPorIdAsync(solicitacao.Id);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(solicitacao.Id, resultado.Id);
        Assert.AreEqual(solicitacao.CursoId, resultado.CursoId);
    }

    [TestMethod]
    public async Task DeveListar_SolicitacoesCertificado()
    {
        List<SolicitacaoCertificado> solicitacoes =
            Builder<SolicitacaoCertificado>
                .CreateListOfSize(3)
                .All()
                .With(x => x.Id = Guid.NewGuid())
                .With(x => x.CursoId = Guid.NewGuid())
                .Build()
                .ToList();

        foreach (SolicitacaoCertificado solicitacao in solicitacoes)
        {
            await repositorioSolicitacaoCertificado
                .CadastrarAsync(solicitacao);
        }

        IReadOnlyList<SolicitacaoCertificado> resultado =
            await repositorioSolicitacaoCertificado
                .SelecionarTodosAsync();

        Assert.AreEqual(3, resultado.Count);
    }

    [TestMethod]
    public async Task DeveAtualizar_SolicitacaoCertificado()
    {
        SolicitacaoCertificado solicitacao =
            Builder<SolicitacaoCertificado>
                .CreateNew()
                .With(x => x.Id = Guid.NewGuid())
                .With(x => x.CursoId = Guid.NewGuid())
                .Build();

        await repositorioSolicitacaoCertificado
            .CadastrarAsync(solicitacao);

        solicitacao.IniciarProcessamento();

        await repositorioSolicitacaoCertificado
            .EditarAsync(solicitacao.Id, solicitacao);

        SolicitacaoCertificado? resultado =
            await repositorioSolicitacaoCertificado
                .SelecionarPorIdAsync(solicitacao.Id);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(
            StatusProcessamento.GerandoCertificados,
            resultado.Status);
    }

    [TestMethod]
    public async Task DeveExcluir_SolicitacaoCertificado()
    {
        SolicitacaoCertificado solicitacao =
            Builder<SolicitacaoCertificado>
                .CreateNew()
                .With(x => x.Id = Guid.NewGuid())
                .With(x => x.CursoId = Guid.NewGuid())
                .Build();

        await repositorioSolicitacaoCertificado
            .CadastrarAsync(solicitacao);

        await repositorioSolicitacaoCertificado
            .ExcluirAsync(solicitacao.Id);

        SolicitacaoCertificado? resultado =
            await repositorioSolicitacaoCertificado
                .SelecionarPorIdAsync(solicitacao.Id);

        Assert.IsNull(resultado);
    }

    [TestMethod]
    public async Task DeveRetornarVerdadeiro_QuandoCursoPossuirProcessamentoEmAndamento()
    {
        Guid cursoId = Guid.NewGuid();

        SolicitacaoCertificado solicitacao =
            Builder<SolicitacaoCertificado>
                .CreateNew()
                .With(x => x.Id = Guid.NewGuid())
                .With(x => x.CursoId = cursoId)
                .Build();

        await repositorioSolicitacaoCertificado
            .CadastrarAsync(solicitacao);

        bool resultado =
            await repositorioSolicitacaoCertificado
                .ExisteProcessamentoEmAndamentoAsync(cursoId);

        Assert.IsTrue(resultado);
    }
}