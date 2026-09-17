using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace GeradorDeCertificados.Testes.Api.Compartilhado;

public abstract class ApiTestBase
{
    protected static WebApplicationFactory<Program> Factory { get; } = new();

    protected HttpClient Client { get; private set; } = null!;

    // Usuários gerenciados pelo Identity
    private readonly List<Guid> usuariosCriados = [];

    // Futuramente:
    // private readonly List<Guid> certificadosCriados = [];
    // private readonly List<Guid> modelosCriados = [];
    // private readonly List<Guid> ... = [];

    [TestInitialize]
    public void Inicializar()
    {
        Client = Factory.CreateClient();
    }

    protected void RegistrarUsuarioCriado(Guid usuarioId)
    {
        usuariosCriados.Add(usuarioId);
    }

    // Futuramente:
    // protected void RegistrarCertificadoCriado(Guid certificadoId)
    // {
    //     certificadosCriados.Add(certificadoId);
    // }

    [TestCleanup]
    public async Task Finalizar()
    {
        Console.WriteLine("=== CLEANUP INICIADO ===");

        Client.Dispose();

        using IServiceScope scope =
            Factory.Services.CreateScope();

        UserManager<IdentityUser<Guid>> userManager =
            scope.ServiceProvider
                .GetRequiredService<UserManager<IdentityUser<Guid>>>();

        // ============================================================
        // IDENTITY
        // ============================================================

        foreach (Guid usuarioId in usuariosCriados)
        {
            Console.WriteLine(
                $"Removendo usuário: {usuarioId}"
            );

            IdentityUser<Guid>? usuario =
                await userManager.FindByIdAsync(
                    usuarioId.ToString()
                );

            if (usuario is not null)
            {
                IdentityResult resultado =
                    await userManager.DeleteAsync(usuario);

                Console.WriteLine(
                    $"Usuário removido: {resultado.Succeeded}"
                );
            }
        }

        // ============================================================
        // ENTIDADES DE DOMÍNIO
        // ============================================================
        //
        // Futuramente, quando existirem entidades de domínio,
        // podemos adicionar aqui os respectivos DbSets/repositórios.
        //
        // Exemplo:
        //
        // foreach (Guid certificadoId in certificadosCriados)
        // {
        //     await dbContext.Database.ExecuteSqlInterpolatedAsync($"""
        //         DELETE FROM "TBCertificados"
        //         WHERE "Id" = {certificadoId};
        //         """);
        // }
        //
        // Se houver relacionamentos entre entidades, a ordem de
        // exclusão deverá respeitar as chaves estrangeiras.
        //
        // ============================================================

        Console.WriteLine("=== CLEANUP FINALIZADO ===");
    }
}