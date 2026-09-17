
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using GeradorDeCertificados.Testes.Api.Compartilhado;
using static GeradorDeCertificados.Api.Modulos.Usuarios.UsuarioContracts;

namespace GeradorDeCertificados.Testes.Api.Modulos.Usuarios;

[TestClass]
[DoNotParallelize]
public sealed class UsuariosApiTests : ApiTestBase
{
    [TestMethod]
    public async Task DeveCadastrar_Usuario()
    {
        // Arrange
        string email = GerarEmail();
        string senha = "Senha@123";

        var request = new CadastrarUsuarioRequest(
            email,
            senha
        );

        // Act
        HttpResponseMessage response =
            await Client.PostAsJsonAsync(
                "/auth/cadastro",
                request
            );

        string corpo =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"STATUS: {(int)response.StatusCode} - {response.StatusCode}"
        );

        Console.WriteLine($"CORPO: {corpo}");

        // Assert
        Assert.AreEqual(
            HttpStatusCode.Created,
            response.StatusCode
        );

        CadastrarUsuarioResponse? resultado =
            await response.Content
                .ReadFromJsonAsync<CadastrarUsuarioResponse>();

        Assert.IsNotNull(resultado);

        Console.WriteLine(
            $"USUÁRIO CRIADO: {resultado.Id}"
        );

        // Registra imediatamente para o cleanup.
        RegistrarUsuarioCriado(resultado.Id);

        Assert.AreNotEqual(
            Guid.Empty,
            resultado.Id
        );

        Assert.AreEqual(
            request.Email,
            resultado.Email
        );
    }


    [TestMethod]
    public async Task DeveFalhar_QuandoEmailJaEstiverCadastrado()
    {
        // Arrange
        string email = GerarEmail();
        string senha = "Senha@123";

        var request = new CadastrarUsuarioRequest(
            email,
            senha
        );

        HttpResponseMessage primeiraResposta =
            await Client.PostAsJsonAsync(
                "/auth/cadastro",
                request
            );

        Assert.AreEqual(
            HttpStatusCode.Created,
            primeiraResposta.StatusCode
        );

        CadastrarUsuarioResponse? usuario =
            await primeiraResposta.Content
                .ReadFromJsonAsync<CadastrarUsuarioResponse>();

        Assert.IsNotNull(usuario);

        // Registra para o cleanup.
        RegistrarUsuarioCriado(usuario.Id);

        // Act
        HttpResponseMessage segundaResposta =
            await Client.PostAsJsonAsync(
                "/auth/cadastro",
                request
            );

        string corpo =
            await segundaResposta.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"STATUS: {(int)segundaResposta.StatusCode} - {segundaResposta.StatusCode}"
        );

        Console.WriteLine($"CORPO: {corpo}");

        // Assert
        Assert.AreEqual(
            HttpStatusCode.Conflict,
            segundaResposta.StatusCode
        );
    }


    [TestMethod]
    public async Task DeveAutenticar_Usuario()
    {
        // Arrange
        string email = GerarEmail();
        string senha = "Senha@123";

        var cadastroRequest = new CadastrarUsuarioRequest(
            email,
            senha
        );

        HttpResponseMessage cadastroResponse =
            await Client.PostAsJsonAsync(
                "/auth/cadastro",
                cadastroRequest
            );

        Assert.AreEqual(
            HttpStatusCode.Created,
            cadastroResponse.StatusCode
        );

        CadastrarUsuarioResponse? cadastro =
            await cadastroResponse.Content
                .ReadFromJsonAsync<CadastrarUsuarioResponse>();

        Assert.IsNotNull(cadastro);

        // Registra antes de continuar o teste.
        RegistrarUsuarioCriado(cadastro.Id);

        var loginRequest = new AutenticarUsuarioRequest(
            cadastroRequest.Email,
            cadastroRequest.Senha
        );

        // Act
        HttpResponseMessage loginResponse =
            await Client.PostAsJsonAsync(
                "/auth/login",
                loginRequest
            );

        string loginCorpo =
            await loginResponse.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"LOGIN: {(int)loginResponse.StatusCode} - {loginResponse.StatusCode}"
        );

        Console.WriteLine(
            $"LOGIN CORPO: {loginCorpo}"
        );

        // Assert
        Assert.AreEqual(
            HttpStatusCode.OK,
            loginResponse.StatusCode
        );

        AutenticarUsuarioResponse? resultado =
            await loginResponse.Content
                .ReadFromJsonAsync<AutenticarUsuarioResponse>();

        Assert.IsNotNull(resultado);

        Assert.AreEqual(
            cadastro.Id,
            resultado.UsuarioId
        );

        Assert.IsFalse(
            string.IsNullOrWhiteSpace(
                resultado.AccessToken
            )
        );

        Assert.IsTrue(
            resultado.DataExpiracaoEmUtc > DateTime.UtcNow
        );
    }


    [TestMethod]
    public async Task DeveFalhar_LoginComEmailInexistente()
    {
        // Arrange
        var request = new AutenticarUsuarioRequest(
            GerarEmail(),
            "Senha@123"
        );

        // Act
        HttpResponseMessage response =
            await Client.PostAsJsonAsync(
                "/auth/login",
                request
            );

        string corpo =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"STATUS: {(int)response.StatusCode} - {response.StatusCode}"
        );

        Console.WriteLine($"CORPO: {corpo}");

        // Assert
        Assert.AreEqual(
            HttpStatusCode.Unauthorized,
            response.StatusCode
        );
    }


    [TestMethod]
    public async Task DeveFalhar_LoginComSenhaIncorreta()
    {
        // Arrange
        string email = GerarEmail();
        string senha = "Senha@123";

        var cadastroRequest = new CadastrarUsuarioRequest(
            email,
            senha
        );

        HttpResponseMessage cadastroResponse =
            await Client.PostAsJsonAsync(
                "/auth/cadastro",
                cadastroRequest
            );

        Assert.AreEqual(
            HttpStatusCode.Created,
            cadastroResponse.StatusCode
        );

        CadastrarUsuarioResponse? cadastro =
            await cadastroResponse.Content
                .ReadFromJsonAsync<CadastrarUsuarioResponse>();

        Assert.IsNotNull(cadastro);

        // Registra para o cleanup.
        RegistrarUsuarioCriado(cadastro.Id);

        var loginRequest = new AutenticarUsuarioRequest(
            email,
            "SenhaIncorreta@123"
        );

        // Act
        HttpResponseMessage response =
            await Client.PostAsJsonAsync(
                "/auth/login",
                loginRequest
            );

        string corpo =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"STATUS: {(int)response.StatusCode} - {response.StatusCode}"
        );

        Console.WriteLine($"CORPO: {corpo}");

        // Assert
        Assert.AreEqual(
            HttpStatusCode.Unauthorized,
            response.StatusCode
        );
    }


    [TestMethod]
    public async Task DeveObter_UsuarioAutenticado()
    {
        // Arrange
        string email = GerarEmail();
        string senha = "Senha@123";

        var cadastroRequest = new CadastrarUsuarioRequest(
            email,
            senha
        );

        HttpResponseMessage cadastroResponse =
            await Client.PostAsJsonAsync(
                "/auth/cadastro",
                cadastroRequest
            );

        Assert.AreEqual(
            HttpStatusCode.Created,
            cadastroResponse.StatusCode
        );

        CadastrarUsuarioResponse? cadastro =
            await cadastroResponse.Content
                .ReadFromJsonAsync<CadastrarUsuarioResponse>();

        Assert.IsNotNull(cadastro);

        // Registra imediatamente para o cleanup.
        RegistrarUsuarioCriado(cadastro.Id);

        var loginRequest = new AutenticarUsuarioRequest(
            email,
            senha
        );

        HttpResponseMessage loginResponse =
            await Client.PostAsJsonAsync(
                "/auth/login",
                loginRequest
            );

        Assert.AreEqual(
            HttpStatusCode.OK,
            loginResponse.StatusCode
        );

        AutenticarUsuarioResponse? login =
            await loginResponse.Content
                .ReadFromJsonAsync<AutenticarUsuarioResponse>();

        Assert.IsNotNull(login);

        Assert.AreEqual(
            cadastro.Id,
            login.UsuarioId
        );

        Assert.IsFalse(
            string.IsNullOrWhiteSpace(
                login.AccessToken
            )
        );

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken
            );

        // Act
        HttpResponseMessage response =
            await Client.GetAsync(
                $"/auth/{cadastro.Id}"
            );

        string corpo =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"GET: {(int)response.StatusCode} - {response.StatusCode}"
        );

        Console.WriteLine($"GET CORPO: {corpo}");

        // Assert
        Assert.AreEqual(
            HttpStatusCode.OK,
            response.StatusCode
        );

        UsuarioResponse? usuario =
            await response.Content
                .ReadFromJsonAsync<UsuarioResponse>();

        Assert.IsNotNull(usuario);

        Assert.AreEqual(
            cadastro.Id,
            usuario.Id
        );

        Assert.AreEqual(
            cadastroRequest.Email,
            usuario.Email
        );
    }


    [TestMethod]
    public async Task DeveRetornar_UnauthorizedAoObterUsuarioSemToken()
    {
        // Arrange
        Guid usuarioId = Guid.CreateVersion7();

        // Act
        HttpResponseMessage response =
            await Client.GetAsync(
                $"/auth/{usuarioId}"
            );

        // Assert
        Assert.AreEqual(
            HttpStatusCode.Unauthorized,
            response.StatusCode
        );
    }


    private static string GerarEmail()
    {
        return $"igor{Guid.NewGuid():N}@email.com";
    }
}
