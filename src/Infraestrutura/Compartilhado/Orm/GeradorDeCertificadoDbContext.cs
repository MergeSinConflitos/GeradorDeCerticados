using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;

public sealed class GeradorDeCertificadoDbContext(
    DbContextOptions<GeradorDeCertificadoDbContext> options
) : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>(options)
{
    // Exemplos dos DbSets que serão adicionados conforme
    // as entidades do projeto forem implementadas.

    // public DbSet<Curso> Cursos => Set<Curso>();
    // public DbSet<SolicitacaoCertificado> Solicitacoes => Set<SolicitacaoCertificado>();
    // public DbSet<Certificado> Certificados => Set<Certificado>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(GeradorDeCertificadoDbContext).Assembly
        );
    }
}