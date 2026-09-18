using GeradorDeCertificados.Dominio.Compartilhado.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;

public sealed class GeradorDeCertificadoDbContext(
    DbContextOptions<GeradorDeCertificadoDbContext> options,
    IProvedorDeUsuario? provedorDeUsuario = null!
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

    //Para manter seguro a interacao do usuario com as entidades futuras
    //Cada usuario so pode mexer nos seus dados
    public override int SaveChanges()
    {
        Guid? usuarioId = provedorDeUsuario?.Id;

        if (!usuarioId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "Não é possível salvar entidades do usuário sem estar autenticado."
            );
        }

        foreach (var entry in ChangeTracker.Entries<IEntidadeDeUsuario>())
        {
            Guid usuarioOriginalId = Guid.Empty;

            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity.UsuarioId == Guid.Empty)
                    {
                        entry.Property(nameof(IEntidadeDeUsuario.UsuarioId)).CurrentValue = usuarioId.Value;
                    }
                    else if (entry.Entity.UsuarioId != usuarioId.Value)
                    {
                        throw new UnauthorizedAccessException(
                            "Tentativa de criar entidade para outro usuário."
                        );
                    }

                    break;

                case EntityState.Modified:
                    usuarioOriginalId = entry
                        .Property(nameof(IEntidadeDeUsuario.UsuarioId))
                        .OriginalValue is Guid idOriginal
                        ? idOriginal
                        : Guid.Empty;

                    Guid idAtualUsuario = entry
                        .Property(nameof(IEntidadeDeUsuario.UsuarioId))
                        .OriginalValue is Guid idAtual
                        ? idAtual
                        : Guid.Empty;

                    if (usuarioOriginalId != idAtualUsuario)
                    {
                        throw new UnauthorizedAccessException(
                              "Não é permitido alterar o usuário de uma entidade."
                          );
                    }

                    if (idAtualUsuario != usuarioId.Value)
                    {
                        throw new UnauthorizedAccessException(
                            "Tentativa de modificar entidade de outro usuário."
                        );
                    }

                    break;

                case EntityState.Deleted:
                    usuarioOriginalId = entry
                        .Property(nameof(IEntidadeDeUsuario.UsuarioId))
                        .OriginalValue is Guid original
                        ? original
                        : Guid.Empty;

                    if (usuarioOriginalId != usuarioId.Value)
                    {
                        throw new UnauthorizedAccessException(
                            "Tentativa de excluir entidade de outro usuário."
                        );
                    }

                    break;
            }
        }

        return base.SaveChanges();
    }
}