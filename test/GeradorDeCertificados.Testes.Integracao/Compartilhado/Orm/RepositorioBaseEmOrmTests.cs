using FizzWare.NBuilder;
using GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;
using GeradorDeCertificados.Testes.Integracao.Compartilhado.Identity;
using Microsoft.EntityFrameworkCore;

public abstract class RepositorioBaseEmOrmTests
{
    protected GeradorDeCertificadoDbContext dbContext = null!;

    // =========================================================
    // REPOSITÓRIOS
    // =========================================================

    // Use de exemplo quando implementar
    // protected RepositorioCursoEmOrm repositorioCurso = null!;


    // =========================================================
    // INICIALIZAÇÃO
    // =========================================================
    [TestInitialize]
    public void InicializarContexto()
    {
        dbContext = CriarDbContext(Guid.NewGuid());

        /*Use de exemplo quando implementar
        repositorioCurso = new RepositorioCursoEmOrm(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Curso>(
            curso => repositorioCurso
                .CadastrarAsync(curso)
                .GetAwaiter()
                .GetResult()
        );

        BuilderSetup.SetCreatePersistenceMethod<IList<Curso>>(
            clientes =>
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
        */
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