
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;
using GeradorDeCertificados.Testes.Api.Compartilhado;
using Microsoft.Extensions.DependencyInjection;
using static GeradorDeCertificados.Api.Modulos.Certificados.CertificadoContracts;
using static GeradorDeCertificados.Api.Modulos.Usuarios.UsuarioContracts;

namespace GeradorDeCertificados.Testes.Api.Modulos.Certificados;

[TestClass]
[DoNotParallelize]
public sealed class CertificadosApiTests : ApiTestBase
{
    private static readonly JsonSerializerOptions OpcoesJson =
        CriarOpcoesJson();

    [TestMethod]
    public async Task DeveSolicitar_Certificados()
    {
        // Arrange
        Guid cursoId = await CriarCurso();

        string email = GerarEmail();
        string senha = "Senha@123";

        var cadastroRequest =
            new CadastrarUsuarioRequest(email, senha);

        using HttpResponseMessage cadastroResponse =
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

        RegistrarUsuarioCriado(cadastro.Id);

        var loginRequest =
            new AutenticarUsuarioRequest(email, senha);

        using HttpResponseMessage loginResponse =
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

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken
            );

        var request =
            new SolicitarCertificadosRequest(
                [
                    "Aluno 1",
                    "Aluno 2"
                ]
            );

        // Act
        using HttpResponseMessage response =
            await Client.PostAsJsonAsync(
                $"/cursos/{cursoId}/certificados",
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
            HttpStatusCode.Accepted,
            response.StatusCode
        );

        SolicitarCertificadosResponse? resultado =
            JsonSerializer.Deserialize<SolicitarCertificadosResponse>(
                corpo,
                OpcoesJson
            );

        Assert.IsNotNull(resultado);

        RegistrarSolicitacaoCriada(resultado.Id);

        Assert.AreNotEqual(
            Guid.Empty,
            resultado.Id
        );

        Assert.AreEqual(
            cursoId,
            resultado.CursoId
        );

        Assert.AreNotEqual(
            default,
            resultado.DataDeCriacao
        );
    }

    [TestMethod]
    public async Task DeveRetornar_ConflitoQuandoHouverProcessamentoEmAndamento()
    {
        // Arrange
        Guid cursoId = await CriarCurso();

        await AutenticarUsuario(
            GerarEmail(),
            "Senha@123"
        );

        var primeiraRequest =
            new SolicitarCertificadosRequest(["Aluno 1"]);

        using HttpResponseMessage primeiraResponse =
            await Client.PostAsJsonAsync(
                $"/cursos/{cursoId}/certificados",
                primeiraRequest
            );

        Assert.AreEqual(
            HttpStatusCode.Accepted,
            primeiraResponse.StatusCode
        );

        SolicitarCertificadosResponse? primeiraSolicitacao =
            await primeiraResponse.Content
                .ReadFromJsonAsync<SolicitarCertificadosResponse>();

        Assert.IsNotNull(primeiraSolicitacao);

        RegistrarSolicitacaoCriada(primeiraSolicitacao.Id);

        var segundaRequest =
            new SolicitarCertificadosRequest(["Aluno 2"]);

        // Act
        using HttpResponseMessage segundaResponse =
            await Client.PostAsJsonAsync(
                $"/cursos/{cursoId}/certificados",
                segundaRequest
            );

        string corpo =
            await segundaResponse.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"STATUS: {(int)segundaResponse.StatusCode} - {segundaResponse.StatusCode}"
        );

        Console.WriteLine($"CORPO: {corpo}");

        // Assert
        Assert.AreEqual(
            HttpStatusCode.Conflict,
            segundaResponse.StatusCode
        );
    }

    [TestMethod]
    public async Task DeveRetornar_UnauthorizedAoSolicitarCertificadosSemToken()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();

        var request =
            new SolicitarCertificadosRequest(["Aluno 1"]);

        // Act
        using HttpResponseMessage response =
            await Client.PostAsJsonAsync(
                $"/cursos/{cursoId}/certificados",
                request
            );

        // Assert
        Assert.AreEqual(
            HttpStatusCode.Unauthorized,
            response.StatusCode
        );
    }

    [TestMethod]
    public async Task DeveObter_StatusDaSolicitacao()
    {
        // Arrange
        Guid cursoId = await CriarCurso();

        await AutenticarUsuario(
            GerarEmail(),
            "Senha@123"
        );

        var request =
            new SolicitarCertificadosRequest(
                [
                    "Aluno 1",
                    "Aluno 2"
                ]
            );

        using HttpResponseMessage solicitacaoResponse =
            await Client.PostAsJsonAsync(
                $"/cursos/{cursoId}/certificados",
                request
            );

        Assert.AreEqual(
            HttpStatusCode.Accepted,
            solicitacaoResponse.StatusCode
        );

        SolicitarCertificadosResponse? solicitacao =
            await solicitacaoResponse.Content
                .ReadFromJsonAsync<SolicitarCertificadosResponse>();

        Assert.IsNotNull(solicitacao);

        RegistrarSolicitacaoCriada(solicitacao.Id);

        // Act
        StatusSolicitacaoResponse resultado =
            await ObterStatusSolicitacao(cursoId);

        // Assert
        Assert.AreEqual(
            cursoId,
            resultado.CursoId
        );

        Assert.AreEqual(
            StatusProcessamento.Concluido,
            resultado.Status
        );
    }

    [TestMethod]
    public async Task DeveListar_Certificados()
    {
        // Arrange
        Guid cursoId = await CriarCurso();

        await AutenticarUsuario(
            GerarEmail(),
            "Senha@123"
        );

        var request =
            new SolicitarCertificadosRequest(
                [
                    "Aluno 1",
                    "Aluno 2"
                ]
            );

        using HttpResponseMessage solicitacaoResponse =
            await Client.PostAsJsonAsync(
                $"/cursos/{cursoId}/certificados",
                request
            );

        Assert.AreEqual(
            HttpStatusCode.Accepted,
            solicitacaoResponse.StatusCode
        );

        SolicitarCertificadosResponse? solicitacao =
            await solicitacaoResponse.Content
                .ReadFromJsonAsync<SolicitarCertificadosResponse>();

        Assert.IsNotNull(solicitacao);

        RegistrarSolicitacaoCriada(solicitacao.Id);

        // Act
        StatusSolicitacaoResponse status =
            await ObterStatusSolicitacao(cursoId);

        Assert.AreEqual(
            StatusProcessamento.Concluido,
            status.Status
        );

        using HttpResponseMessage response =
            await Client.GetAsync(
                $"/cursos/{cursoId}/certificados"
            );

        string corpo =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"STATUS: {(int)response.StatusCode} - {response.StatusCode}"
        );

        Console.WriteLine($"CORPO: {corpo}");

        // Assert
        Assert.AreEqual(
            HttpStatusCode.OK,
            response.StatusCode
        );

        List<CertificadoResponse>? resultado =
            JsonSerializer.Deserialize<List<CertificadoResponse>>(
                corpo,
                OpcoesJson
            );

        Assert.IsNotNull(resultado);

        Assert.HasCount(2, resultado);

        CollectionAssert.AreEquivalent(
            new[] { "Aluno 1", "Aluno 2" },
            resultado.Select(x => x.NomeAluno).ToArray()
        );

        Assert.AreEqual(
            cursoId,
            resultado[0].CursoId
        );

        Assert.AreEqual(
            StatusCertificado.Gerado,
            resultado[0].Status
        );
    }

    [TestMethod]
    public async Task DeveRetornar_UnauthorizedAoListarCertificadosSemToken()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();

        // Act
        using HttpResponseMessage response =
            await Client.GetAsync(
                $"/cursos/{cursoId}/certificados"
            );

        // Assert
        Assert.AreEqual(
            HttpStatusCode.Unauthorized,
            response.StatusCode
        );
    }

    [TestMethod]
    public async Task DeveBaixar_Certificados()
    {
        // Arrange
        Guid cursoId = await CriarCurso();

        await AutenticarUsuario(
            GerarEmail(),
            "Senha@123"
        );

        var request =
            new SolicitarCertificadosRequest(
                [
                    "Aluno 1",
                    "Aluno 2"
                ]
            );

        using HttpResponseMessage solicitacaoResponse =
            await Client.PostAsJsonAsync(
                $"/cursos/{cursoId}/certificados",
                request
            );

        Assert.AreEqual(
            HttpStatusCode.Accepted,
            solicitacaoResponse.StatusCode
        );

        SolicitarCertificadosResponse? solicitacao =
            await solicitacaoResponse.Content
                .ReadFromJsonAsync<SolicitarCertificadosResponse>();

        Assert.IsNotNull(solicitacao);

        RegistrarSolicitacaoCriada(solicitacao.Id);

        StatusSolicitacaoResponse status =
            await ObterStatusSolicitacao(cursoId);

        Assert.AreEqual(
            StatusProcessamento.Concluido,
            status.Status
        );

        // Act
        using HttpResponseMessage response =
            await Client.GetAsync(
                $"/cursos/{cursoId}/certificados/download"
            );

        // Assert
        Assert.AreEqual(
            HttpStatusCode.OK,
            response.StatusCode
        );

        Assert.AreEqual(
            "application/zip",
            response.Content.Headers.ContentType?.MediaType
        );

        byte[] conteudo =
            await response.Content.ReadAsByteArrayAsync();

        Assert.IsNotEmpty(conteudo);

        Assert.IsTrue(
            response.Content.Headers.ContentDisposition?
                .FileName?
                .Contains("certificados") == true
        );
    }

    [TestMethod]
    public async Task DeveRetornar_NaoEncontradoAoBaixarCertificadosDeCursoSemSolicitacao()
    {
        // Arrange
        Guid cursoId = await CriarCurso();

        await AutenticarUsuario(
            GerarEmail(),
            "Senha@123"
        );

        // Act
        using HttpResponseMessage response =
            await Client.GetAsync(
                $"/cursos/{cursoId}/certificados/download"
            );

        string corpo =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"STATUS: {(int)response.StatusCode} - {response.StatusCode}"
        );

        Console.WriteLine($"CORPO: {corpo}");

        // Assert
        Assert.AreEqual(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    [TestMethod]
    public async Task DeveRetornar_UnauthorizedAoBaixarCertificadosSemToken()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();

        // Act
        using HttpResponseMessage response =
            await Client.GetAsync(
                $"/cursos/{cursoId}/certificados/download"
            );

        // Assert
        Assert.AreEqual(
            HttpStatusCode.Unauthorized,
            response.StatusCode
        );
    }

    private async Task<StatusSolicitacaoResponse> ObterStatusSolicitacao(
        Guid cursoId)
    {
        for (int tentativa = 0; tentativa < 50; tentativa++)
        {
            using HttpResponseMessage response =
                await Client.GetAsync(
                    $"/cursos/{cursoId}/status"
                );

            Assert.AreEqual(
                HttpStatusCode.OK,
                response.StatusCode
            );

            StatusSolicitacaoResponse? resultado =
                await response.Content
                    .ReadFromJsonAsync<StatusSolicitacaoResponse>(
                        OpcoesJson
                    );

            Assert.IsNotNull(resultado);

            if (resultado.Status == StatusProcessamento.Concluido ||
                resultado.Status == StatusProcessamento.Falha)
            {
                return resultado;
            }

            await Task.Delay(200);
        }

        Assert.Fail(
            $"O processamento do curso {cursoId} não terminou dentro do tempo esperado."
        );

        throw new InvalidOperationException();
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
                DateTimeKind.Utc
            )
        );

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
                senha
            );

        using HttpResponseMessage cadastroResponse =
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

        RegistrarUsuarioCriado(cadastro.Id);

        var loginRequest =
            new AutenticarUsuarioRequest(
                email,
                senha
            );

        using HttpResponseMessage loginResponse =
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

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken
            );
    }

    private static JsonSerializerOptions CriarOpcoesJson()
    {
        JsonSerializerOptions opcoes =
            new(JsonSerializerDefaults.Web);

        opcoes.Converters.Add(
            new JsonStringEnumConverter()
        );

        return opcoes;
    }

    private static string GerarEmail()
    {
        return $"igor{Guid.NewGuid():N}@email.com";
    }
}
