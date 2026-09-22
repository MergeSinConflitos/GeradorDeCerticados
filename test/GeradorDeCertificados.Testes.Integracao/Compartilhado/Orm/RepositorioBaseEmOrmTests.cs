using FizzWare.NBuilder;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;
using GeradorDeCertificados.Infraestrutura.Modulos.Certificados;
using GeradorDeCertificados.Infraestrutura.Modulos.Cursos;
using GeradorDeCertificados.Testes.Integracao.Compartilhado.Identity;
using Microsoft.EntityFrameworkCore;

public abstract class RepositorioBaseEmOrmTests
{
    protected GeradorDeCertificadoDbContext dbContext = null!;

    // =========================================================
    // REPOSITÓRIOS
    // =========================================================

    // Use de exemplo quando implementar
    protected RepositorioCursoEmOrm repositorioCurso = null!;
    protected RepositorioCertificadoEmOrm repositorioCertificado = null!;
    protected RepositorioSolicitacaoCertificadoEmOrm repositorioSolicitacaoCertificado = null!;


    // =========================================================
    // INICIALIZAÇÃO
    // =========================================================
    [TestInitialize]
    public void InicializarContexto()
    {
        dbContext = CriarDbContext(Guid.NewGuid());

        //Use de exemplo quando implementar
        repositorioCurso = new RepositorioCursoEmOrm(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Curso>(
            curso => repositorioCurso
                .CadastrarAsync(curso)
                .GetAwaiter()
                .GetResult()
        );

        BuilderSetup.SetCreatePersistenceMethod<IList<Curso>>(
            cursos =>
            {
                foreach (Curso curso in cursos)
                {
                    repositorioCurso
                        .CadastrarAsync(curso)
                        .GetAwaiter()
                        .GetResult();
                }
            }
        );


        //==================================================
        // CERTIFICADO
        //==================================================

        repositorioCertificado = new RepositorioCertificadoEmOrm(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Certificado>(
            certificado => repositorioCertificado
            .CadastrarAsync(certificado)
            .GetAwaiter()
            .GetResult()
        );

        BuilderSetup.SetCreatePersistenceMethod<IList<Certificado>>(
            certificados =>
            {
                foreach (Certificado certificado in certificados)
                {
                    repositorioCertificado
                    .CadastrarAsync(certificado)
                    .GetAwaiter()
                    .GetResult();
                }
            }
        );

        //=====================================================
        // SOLICITACAO CERTIFICADO
        //=====================================================

        repositorioSolicitacaoCertificado =
    new RepositorioSolicitacaoCertificadoEmOrm(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<SolicitacaoCertificado>(
        solicitacao => repositorioSolicitacaoCertificado
            .CadastrarAsync(solicitacao)
            .GetAwaiter()
            .GetResult()
    );

        BuilderSetup.SetCreatePersistenceMethod<IList<SolicitacaoCertificado>>(
        solicitacoes =>
        {
            foreach (SolicitacaoCertificado solicitacao in solicitacoes)
            {
                repositorioSolicitacaoCertificado
                    .CadastrarAsync(solicitacao)
                    .GetAwaiter()
                    .GetResult();
            }
        }
    );
    }

    // =========================================================
    // LIMPEZA
    // =========================================================

    [TestCleanup]
    public void DescartarContexto()
    {
        dbContext.Dispose();
    }


    // =========================================================
    // CRIAÇÃO DO CONTEXTO
    // =========================================================

    private static GeradorDeCertificadoDbContext CriarDbContext(
        Guid userId
    )
    {
        DbContextOptions<GeradorDeCertificadoDbContext> options =
            new DbContextOptionsBuilder<GeradorDeCertificadoDbContext>()
                .UseInMemoryDatabase(
                    $"integracao-{Guid.NewGuid():N}"
                )
                .Options;

        return new GeradorDeCertificadoDbContext(
            options,
            new ProvedorDeUsuarioFake(userId)
        );
    }
}