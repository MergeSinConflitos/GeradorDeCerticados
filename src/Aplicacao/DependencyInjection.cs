using GeradorDeCertificados.Aplicacao.Modulos.Certificados;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeradorDeCertificados.Aplicacao;

public static class DependencyInjection
{
    public static void AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(
                typeof(DependencyInjection).Assembly
            );
        });

        var rabbitMqConnectionString =
            configuration.GetConnectionString("RabbitMq")
            ?? throw new InvalidOperationException(
                "A ConnectionString \"RabbitMq\" não foi configurada."
            );

        services.AddMassTransit(config =>
        {   

            //Consummer
            config.AddConsumer<GerarCertificadosConsumer>();


            config.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(rabbitMqConnectionString));

                cfg.ReceiveEndpoint("certificados-created", endpoint =>{ 
                    endpoint.ConfigureConsumer<GerarCertificadosConsumer>(context);
                });
            });
        });

        services.Configure<MassTransitHostOptions>(options =>
        {
            options.WaitUntilStarted = true;
            options.StartTimeout = TimeSpan.FromSeconds(30);
        });

        services.AddSingleton<GeradorPdfCertificado>();
    }
}