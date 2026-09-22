
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using GeradorDeCertificados.Api.Modulos.Cursos;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;
using GeradorDeCertificados.Testes.Api.Compartilhado;
using Microsoft.Extensions.DependencyInjection;
using static GeradorDeCertificados.Api.Modulos.Usuarios.UsuarioContracts;

namespace GeradorDeCertificados.Testes.Api.Modulos.Cursos;

[TestClass]
[DoNotParallelize]
public sealed class CursosApiTests : ApiTestBase
{
    [TestMethod]
    public async Task DeveCadastrar_Curso()
    {
        // Arrange
        string email = GerarEmail();
        string senha = "Senha@123";

        await AutenticarUsuario(
            email,
            senha);

        var request = new CadastrarCursoRequest(
            "Curso de C#",
            "Curso completo de C# e .NET",
            40,
            new DateTime(
                2026,
                9,
                20,
                0,
                0,
                0,
                DateTimeKind.Utc));

        // Act
        HttpResponseMessage response =
            await Client.PostAsJsonAsync(
                "/Api/cursos",
                request);

        string corpo =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"STATUS: {(int)response.StatusCode} - {response.StatusCode}");

        Console.WriteLine($"CORPO: {corpo}");

        // Assert
        Assert.AreEqual(
            HttpStatusCode.Created,
            response.StatusCode);

        CadastrarCursoResponse? resultado =
            await response.Content
                .ReadFromJsonAsync<CadastrarCursoResponse>();

        Assert.IsNotNull(resultado);

        Assert.AreNotEqual(
            Guid.Empty,
            resultado.CursoId);

        RegistrarCursoCriado(resultado.CursoId);
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoCursoForInvalido()
    {
        // Arrange
        string email = GerarEmail();
        string senha = "Senha@123";

        await AutenticarUsuario(
            email,
            senha);

        var request = new CadastrarCursoRequest(
            string.Empty,
            "Curso completo de C# e .NET",
            40,
            new DateTime(
                2026,
                9,
                20,
                0,
                0,
                0,
                DateTimeKind.Utc));

        // Act
        HttpResponseMessage response =
            await Client.PostAsJsonAsync(
                "/Api/cursos",
                request);

        string corpo =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"STATUS: {(int)response.StatusCode} - {response.StatusCode}");

        Console.WriteLine($"CORPO: {corpo}");

        // Assert
        Assert.AreEqual(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [TestMethod]
    public async Task DeveRetornar_UnauthorizedAoCadastrarCursoSemToken()
    {
        // Arrange
        var request = new CadastrarCursoRequest(
            "Curso de C#",
            "Curso completo de C# e .NET",
            40,
            new DateTime(
                2026,
                9,
                20,
                0,
                0,
                0,
                DateTimeKind.Utc));

        // Act
        HttpResponseMessage response =
            await Client.PostAsJsonAsync(
                "/Api/cursos",
                request);

        // Assert
        Assert.AreEqual(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [TestMethod]
    public async Task DeveObter_CursoPorId()
    {
        // Arrange
        Guid cursoId = await CriarCurso();

        string email = GerarEmail();
        string senha = "Senha@123";

        await AutenticarUsuario(
            email,
            senha);

        // Act
        HttpResponseMessage response =
            await Client.GetAsync(
                $"/Api/cursos/{cursoId}");

        string corpo =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"STATUS: {(int)response.StatusCode} - {response.StatusCode}");

        Console.WriteLine($"CORPO: {corpo}");

        // Assert
        Assert.AreEqual(
            HttpStatusCode.OK,
            response.StatusCode);

        CursoResponse? resultado =
            await response.Content
                .ReadFromJsonAsync<CursoResponse>();

        Assert.IsNotNull(resultado);

        Assert.AreEqual(
            cursoId,
            resultado.Id);

        Assert.AreEqual(
            "Curso de C#",
            resultado.Nome);

        Assert.AreEqual(
            "Curso completo de C# e .NET",
            resultado.Descricao);

        Assert.AreEqual(
            40,
            resultado.CargaHoraria);

        Assert.AreEqual(
            new DateTime(
                2026,
                9,
                20,
                0,
                0,
                0,
                DateTimeKind.Utc),
            resultado.DataConclusao);
    }

    [TestMethod]
    public async Task DeveRetornar_NaoEncontradoQuandoCursoNaoExistir()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();

        string email = GerarEmail();
        string senha = "Senha@123";

        await AutenticarUsuario(
            email,
            senha);

        // Act
        HttpResponseMessage response =
            await Client.GetAsync(
                $"/Api/cursos/{cursoId}");

        string corpo =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"STATUS: {(int)response.StatusCode} - {response.StatusCode}");

        Console.WriteLine($"CORPO: {corpo}");

        // Assert
        Assert.AreEqual(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [TestMethod]
    public async Task DeveRetornar_UnauthorizedAoObterCursoSemToken()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();

        // Act
        HttpResponseMessage response =
            await Client.GetAsync(
                $"/Api/cursos/{cursoId}");

        // Assert
        Assert.AreEqual(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    private async Task<Guid> CriarCurso()
    {
        Guid cursoId = Guid.CreateVersion7();

        Curso curso = new(
            cursoId,
            "Curso de C#",
            "Curso completo de C# e .NET",
            40,
            new DateTime(
                2026,
                9,
                20,
                0,
                0,
                0,
                DateTimeKind.Utc));

        using IServiceScope scope =
            Factory.Services.CreateScope();

        GeradorDeCertificadoDbContext dbContext =
            scope.ServiceProvider
                .GetRequiredService<GeradorDeCertificadoDbContext>();

        dbContext.Set<Curso>().Add(curso);

        await dbContext.SaveChangesAsync();

        RegistrarCursoCriado(cursoId);

        return cursoId;
    }

    private async Task AutenticarUsuario(
        string email,
        string senha)
    {
        var cadastroRequest =
            new CadastrarUsuarioRequest(
                email,
                senha);

        HttpResponseMessage cadastroResponse =
            await Client.PostAsJsonAsync(
                "/auth/cadastro",
                cadastroRequest);

        Assert.AreEqual(
            HttpStatusCode.Created,
            cadastroResponse.StatusCode);

        CadastrarUsuarioResponse? cadastro =
            await cadastroResponse.Content
                .ReadFromJsonAsync<CadastrarUsuarioResponse>();

        Assert.IsNotNull(cadastro);

        RegistrarUsuarioCriado(cadastro.Id);

        var loginRequest =
            new AutenticarUsuarioRequest(
                email,
                senha);

        HttpResponseMessage loginResponse =
            await Client.PostAsJsonAsync(
                "/auth/login",
                loginRequest);

        Assert.AreEqual(
            HttpStatusCode.OK,
            loginResponse.StatusCode);

        AutenticarUsuarioResponse? login =
            await loginResponse.Content
                .ReadFromJsonAsync<AutenticarUsuarioResponse>();

        Assert.IsNotNull(login);

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken);
    }

    private static string GerarEmail()
    {
        return $"igor{Guid.NewGuid():N}@email.com";
    }
}