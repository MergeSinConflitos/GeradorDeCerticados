using System.Diagnostics;
using GeradorDeCertificados.Api.Compartilhado.Auth;
using GeradorDeCertificados.Api.Compartilhado.Http;
using GeradorDeCertificados.Api.Compartilhado.Logging;
using GeradorDeCertificados.Infraestrutura;
using GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;
using GeradorDeCertificados.Aplicacao;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


// ============================================================
// CONFIGURAÇÃO DO JWT
// ============================================================
// Lê as configurações da seção "Jwt" do appsettings.json.
//
// Exemplo:
//
// "Jwt": {
//     "Issuer": "...",
//     "Audience": "...",
//     "Key": "...",
//     "AccessTokenMinutes": 60
// }
//
// ValidateOnStart() faz a aplicação verificar essas configurações
// durante a inicialização, em vez de descobrir um erro somente
// quando alguém tentar fazer login.
builder.Services
    .AddOptions<JwtOptions>()
    .BindConfiguration(JwtOptions.SectionName)
    .Validate(o => !string.IsNullOrWhiteSpace(o.Issuer))
    .Validate(o => !string.IsNullOrWhiteSpace(o.Audience))
    .Validate(o => !string.IsNullOrWhiteSpace(o.Key))
    .ValidateOnStart();


// Configura como o ASP.NET Core deve validar os tokens JWT
// recebidos nas requisições.
//
// A configuração detalhada fica concentrada no JwtExtensions.
builder.Services
    .AddOptions<JwtBearerOptions>(
        JwtBearerDefaults.AuthenticationScheme
    )
    .Configure<IOptions<JwtOptions>>(
        JwtExtensions.ConfigureJwtBearerValidation
    );


// ============================================================
// INFRAESTRUTURA
// ============================================================
// Registra:
// - Entity Framework Core
// - PostgreSQL
// - ASP.NET Core Identity
// - UserManager
// - Repositórios
// - Gerenciador de identidade
// - Outras dependências da infraestrutura.
builder.Services.AddInfrastructureServices(builder.Configuration);


// ============================================================
// APLICAÇÃO
// ============================================================
// Registra:
// - MediatR
// - Commands
// - Queries
// - Handlers
// - MassTransit
// - RabbitMQ
builder.Services.AddApplicationServices(builder.Configuration);


// ============================================================
// AUTENTICAÇÃO JWT
// ============================================================
// Configura o ASP.NET Core para reconhecer:
//
// Authorization: Bearer {token}
//
// enviado nas requisições protegidas.
builder.Services.AddJwtAuthServices();


// ============================================================
// LOGGING
// ============================================================
// Configura o Serilog para registrar os logs da aplicação.
builder.Services.AddSerilogServices(
    builder.Logging
);


// ============================================================
// CONTROLLERS
// ============================================================
// Habilita Controllers da API.
//
// ConfigureApiBehaviorOptions permite personalizar as respostas
// automáticas de erro geradas pelo ASP.NET Core.
builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.ClientErrorMapping[
            StatusCodes.Status400BadRequest
        ].Link = ProblemDetailsTypes.BadRequest;

        options.ClientErrorMapping[
            StatusCodes.Status401Unauthorized
        ].Link = ProblemDetailsTypes.Unauthorized;

        options.ClientErrorMapping[
            StatusCodes.Status403Forbidden
        ].Link = ProblemDetailsTypes.Forbidden;

        options.ClientErrorMapping[
            StatusCodes.Status404NotFound
        ].Link = ProblemDetailsTypes.NotFound;

        options.ClientErrorMapping[
            StatusCodes.Status409Conflict
        ].Link = ProblemDetailsTypes.Conflict;
    });

builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add
    (new JsonStringEnumConverter());
});
// ============================================================
// PROBLEM DETAILS
// ============================================================
// Padroniza respostas de erro da API.
//
// Em vez de cada Controller precisar montar manualmente
// respostas de erro diferentes, a API utiliza o padrão
// ProblemDetails.
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        string? type =
            ProblemDetailsTypes.ObterPorStatus(
                context.ProblemDetails.Status
            );

        if (type is not null)
        {
            context.ProblemDetails.Type = type;
        }


        // Personaliza a resposta de erro 401.
        if (context.ProblemDetails.Status ==
            StatusCodes.Status401Unauthorized)
        {
            context.ProblemDetails.Title =
                "Não Autenticado";

            context.ProblemDetails.Detail =
                "É necessário fornecer credenciais válidas.";
        }


        // Personaliza a resposta de erro 403.
        else if (context.ProblemDetails.Status ==
                 StatusCodes.Status403Forbidden)
        {
            context.ProblemDetails.Title =
                "Acesso Negado";

            context.ProblemDetails.Detail =
                "O usuário autenticado não tem permissão para acessar este recurso.";
        }


        // Identificador da requisição.
        //
        // É útil para encontrar nos logs exatamente qual requisição
        // produziu determinado erro.
        context.ProblemDetails.Extensions["traceId"] =
            Activity.Current?.Id ??
            context.HttpContext.TraceIdentifier;
    };
});


// ============================================================
// OPENAPI / SWAGGER
// ============================================================
// Registra a documentação OpenAPI.
builder.Services.AddOpenApi();


// Configura o Swagger para permitir autenticação através
// do botão "Authorize".
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,

            Description =
                "Informe o token JWT no formato: Bearer {token}"
        }
    );


    // Informa ao Swagger que os endpoints podem utilizar
    // autenticação Bearer.
    options.AddSecurityRequirement(
        document =>
            new OpenApiSecurityRequirement
            {
                [
                    new OpenApiSecuritySchemeReference(
                        "Bearer",
                        document,
                        null
                    )
                ] = []
            }
    );
});


// ============================================================
// CONSTRUÇÃO DA APLICAÇÃO
// ============================================================
GeradorPdfCertificado.ConfiguracaoQuestPdf();
var app = builder.Build();


// ============================================================
// AMBIENTE DE DESENVOLVIMENTO
// ============================================================
// Durante o desenvolvimento:
// - aplica migrations pendentes;
// - disponibiliza Swagger.
//
// Em produção, normalmente não queremos executar
// Database.Migrate() automaticamente dessa maneira.
if (app.Environment.IsDevelopment())
{
    using IServiceScope scope =
        app.Services.CreateScope();

    GeradorDeCertificadoDbContext dbContext =
        scope.ServiceProvider
            .GetRequiredService<GeradorDeCertificadoDbContext>();

    dbContext.Database.Migrate();


    app.UseSwagger();
    app.UseSwaggerUI();
}


// ============================================================
// TRATAMENTO DE ERROS
// ============================================================
// Captura exceções não tratadas e transforma em respostas
// HTTP padronizadas.
app.UseExceptionHandler();


// Permite gerar respostas para códigos HTTP que não passaram
// por uma exceção.
app.UseStatusCodePages();


// ============================================================
// HTTPS
// ============================================================
// Redireciona requisições HTTP para HTTPS.
app.UseHttpsRedirection();


// ============================================================
// AUTENTICAÇÃO E AUTORIZAÇÃO
// ============================================================
// Authentication:
// verifica se o JWT enviado é válido.
//
// Authorization:
// verifica se o usuário pode acessar determinado recurso.
app.UseAuthentication();
app.UseAuthorization();


// ============================================================
// CONTROLLERS
// ============================================================
// Mapeia os endpoints dos Controllers.
app.MapControllers();


// Inicia a aplicação.
app.Run();