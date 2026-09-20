using GeradorDeCertificados.Dominio.Compartilhado.Auth;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using GeradorDeCertificados.Infraestrutura.Compartilhado.Auth;
using GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;
using GeradorDeCertificados.Infraestrutura.Modulos.Certificados;
using GeradorDeCertificados.Infraestrutura.Modulos.Cursos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeradorDeCertificados.Infraestrutura;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddScoped<IRepositorioCurso, RepositorioCursoEmOrm>();
        services.AddScoped<IRepositorioCertificado, RepositorioCertificadoEmOrm>();
        services.AddScoped<IRepositorioSolicitacaoCertificado, RepositorioSolicitacaoCertificadoEmOrm>();

        services.AddDataProtection();
        services.AddIdentityCore<IdentityUser<Guid>>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;

            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        })
        .AddEntityFrameworkStores<GeradorDeCertificadoDbContext>();

        services.AddDbContext<GeradorDeCertificadoDbContext>(options =>
        {
            if (configuration["Infra:DatabaseProvider"] == "InMemory")
            {
                options.UseInMemoryDatabase("GeradorDeCertificados");
            }
            else
            {
                string? connectionString =
                    configuration.GetConnectionString("PostgresEF");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException(
                        "A connection string \"PostgresEF\" não foi encontrada."
                    );
                }

                options.UseNpgsql(connectionString, opt =>
                {
                    opt.EnableRetryOnFailure(3);
                });
            }
        });

        services.AddScoped<
            IGerenciadorDeIdentidade,
            GerenciadorDeIdentidade
        >();
    }
}