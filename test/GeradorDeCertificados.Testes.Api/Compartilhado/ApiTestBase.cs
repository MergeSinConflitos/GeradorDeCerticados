using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace GeradorDeCertificados.Testes.Api.Compartilhado;

public abstract class ApiTestBase
{
    protected static WebApplicationFactory<Program> Factory { get; } = new();

    protected HttpClient Client { get; private set; } = null!;

    // Usuários gerenciados pelo Identity
    private readonly List<Guid> usuariosCriados = [];

    // Entidades de domínio
    private readonly List<Guid> cursosCriados = [];
    private readonly List<Guid> solicitacoesCriadas = [];
    private readonly List<Guid> certificadosCriados = [];

    [TestInitialize]
    public void Inicializar()
    {
        Client = Factory.CreateClient();
    }

    protected void RegistrarUsuarioCriado(Guid usuarioId)
    {
        usuariosCriados.Add(usuarioId);
    }

    protected void RegistrarCursoCriado(Guid cursoId)
    {
        cursosCriados.Add(cursoId);
    }

    protected void RegistrarSolicitacaoCriada(Guid solicitacaoId)
    {
        solicitacoesCriadas.Add(solicitacaoId);
    }

    protected void RegistrarCertificadoCriado(Guid certificadoId)
    {
        certificadosCriados.Add(certificadoId);
    }

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

        GeradorDeCertificadoDbContext dbContext =
            scope.ServiceProvider
                .GetRequiredService<GeradorDeCertificadoDbContext>();

        // ============================================================
        // ENTIDADES DE DOMÍNIO
        // ============================================================

        foreach (Guid certificadoId in certificadosCriados)
        {
            Console.WriteLine(
                $"Removendo certificado: {certificadoId}"
            );

            await dbContext.Set<Certificado>()
                .Where(x => x.Id == certificadoId)
                .ExecuteDeleteAsync();
        }

        foreach (Guid solicitacaoId in solicitacoesCriadas)
        {
            Console.WriteLine(
                $"Removendo solicitação: {solicitacaoId}"
            );

            await dbContext.Set<SolicitacaoCertificado>()
                .Where(x => x.Id == solicitacaoId)
                .ExecuteDeleteAsync();
        }

        foreach (Guid cursoId in cursosCriados)
        {
            Console.WriteLine(
                $"Removendo curso: {cursoId}"
            );

            await dbContext.Set<Curso>()
                .Where(x => x.Id == cursoId)
                .ExecuteDeleteAsync();
        }

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

        Console.WriteLine("=== CLEANUP FINALIZADO ===");
    }
}