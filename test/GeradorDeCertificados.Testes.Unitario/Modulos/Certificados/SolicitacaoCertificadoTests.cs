using GeradorDeCertificados.Dominio.Compartilhado;
using GeradorDeCertificados.Dominio.Modulos.Certificados;

namespace GeradorDeCertificados.Testes.Unitario.Modulos.Certificados;

[TestClass]
public sealed class SolicitacaoCertificadoTests
{
    [TestMethod]
    public void DeveRetornar_ErroQuandoCursoNaoForInformado()
    {
        // Arrange
        List<Certificado> certificados =
        [
            CriarCertificado()
        ];

        SolicitacaoCertificado solicitacao = new(
            Guid.NewGuid(),
            Guid.Empty,
            certificados);

        // Act
        IReadOnlyList<ErroValidacao> erros = solicitacao.Validar();

        // Assert
        Assert.IsTrue(erros.Any());
        Assert.AreEqual(
            nameof(SolicitacaoCertificado.CursoId),
            erros[0].Campo);
    }

    [TestMethod]
    public void DeveRetornar_ErroQuandoNaoPossuirCertificados()
    {
        // Arrange
        SolicitacaoCertificado solicitacao = new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            []);

        // Act
        IReadOnlyList<ErroValidacao> erros = solicitacao.Validar();

        // Assert
        Assert.IsTrue(erros.Any());
        Assert.AreEqual(
            nameof(SolicitacaoCertificado.Certificados),
            erros[0].Campo);
    }

    [TestMethod]
    public void DeveCriar_SolicitacaoComStatusPendente()
    {
        // Arrange
        SolicitacaoCertificado solicitacao = CriarSolicitacao();

        // Act
        StatusProcessamento status = solicitacao.Status;

        // Assert
        Assert.AreEqual(
            StatusProcessamento.Pendente,
            status);
    }

    [TestMethod]
    public void DeveIniciar_ProcessamentoDeCertificados()
    {
        // Arrange
        SolicitacaoCertificado solicitacao = CriarSolicitacao();

        // Act
        solicitacao.IniciarProcessamento();

        // Assert
        Assert.AreEqual(
            StatusProcessamento.GerandoCertificados,
            solicitacao.Status);
    }

    [TestMethod]
    public void DeveIniciar_GeracaoDoZip()
    {
        // Arrange
        SolicitacaoCertificado solicitacao = CriarSolicitacao();

        // Act
        solicitacao.IniciarGeracaoZip();

        // Assert
        Assert.AreEqual(
            StatusProcessamento.GerandoZip,
            solicitacao.Status);
    }

    [TestMethod]
    public void DeveConcluir_Solicitacao()
    {
        // Arrange
        SolicitacaoCertificado solicitacao = CriarSolicitacao();

        string caminhoZip = "certificados/curso.zip";

        // Act
        solicitacao.Concluir(caminhoZip);

        // Assert
        Assert.AreEqual(
            StatusProcessamento.Concluido,
            solicitacao.Status);

        Assert.AreEqual(
            caminhoZip,
            solicitacao.CaminhoZip);

        Assert.IsNotNull(solicitacao.DataDeConclusao);
    }

    [TestMethod]
    public void DeveMarcar_SolicitacaoComoFalha()
    {
        // Arrange
        SolicitacaoCertificado solicitacao = CriarSolicitacao();

        // Act
        solicitacao.MarcarComoFalha();

        // Assert
        Assert.AreEqual(
            StatusProcessamento.Falha,
            solicitacao.Status);
    }

    private static SolicitacaoCertificado CriarSolicitacao()
    {
        List<Certificado> certificados =
        [
            CriarCertificado()
        ];

        return new SolicitacaoCertificado(
            Guid.NewGuid(),
            Guid.NewGuid(),
            certificados);
    }

    private static Certificado CriarCertificado()
    {
        return new Certificado(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "João da Silva");
    }
}