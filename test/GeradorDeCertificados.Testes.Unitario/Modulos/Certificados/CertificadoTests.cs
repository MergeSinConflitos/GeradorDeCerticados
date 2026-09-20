using GeradorDeCertificados.Dominio.Compartilhado;
using GeradorDeCertificados.Dominio.Modulos.Certificados;

namespace GeradorDeCertificados.Testes.Unitario.Modulos.Certificados;

[TestClass]
public sealed class CertificadoTests
{
    [TestMethod]
    public void DeveRetornar_ErroQuandoNomeAlunoNaoForInformado()
    {
        // Arrange
        Certificado certificado = new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            string.Empty);

        // Act
        IReadOnlyList<ErroValidacao> erros = certificado.Validar();

        // Assert
        Assert.IsTrue(erros.Any());
        Assert.AreEqual(nameof(Certificado.NomeAluno), erros[0].Campo);
    }

    [TestMethod]
    public void DeveRetornar_ErroQuandoNomeAlunoPossuirMaisDe200Caracteres()
    {
        // Arrange
        string nomeAluno = new('A', 201);

        Certificado certificado = new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            nomeAluno);

        // Act
        IReadOnlyList<ErroValidacao> erros = certificado.Validar();

        // Assert
        Assert.IsTrue(erros.Any());
        Assert.AreEqual(nameof(Certificado.NomeAluno), erros[0].Campo);
    }

    [TestMethod]
    public void DeveRetornar_ErroQuandoCursoNaoForInformado()
    {
        // Arrange
        Certificado certificado = new(
            Guid.NewGuid(),
            Guid.Empty,
            Guid.NewGuid(),
            "João da Silva");

        // Act
        IReadOnlyList<ErroValidacao> erros = certificado.Validar();

        // Assert
        Assert.IsTrue(erros.Any());
        Assert.AreEqual(nameof(Certificado.CursoId), erros[0].Campo);
    }

    [TestMethod]
    public void DeveCriar_CertificadoComStatusPendente()
    {
        // Arrange
        Certificado certificado = new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "João da Silva");

        // Act
        StatusCertificado status = certificado.Status;

        // Assert
        Assert.AreEqual(StatusCertificado.Pendente, status);
    }

    [TestMethod]
    public void DeveMarcar_CertificadoComoGerado()
    {
        // Arrange
        Certificado certificado = new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "João da Silva");

        string caminhoArquivo = "certificados/joao.pdf";
        DateOnly dataGeracao = new(2026, 9, 20);

        // Act
        certificado.MarcarComoGerado(
            caminhoArquivo,
            dataGeracao);

        // Assert
        Assert.AreEqual(StatusCertificado.Gerado, certificado.Status);
        Assert.AreEqual(caminhoArquivo, certificado.CaminhoArquivo);
        Assert.AreEqual(dataGeracao, certificado.DataDeGeracao);
    }

    [TestMethod]
    public void DeveMarcar_CertificadoComoFalha()
    {
        // Arrange
        Certificado certificado = new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "João da Silva");

        // Act
        certificado.MarcarComoFalha();

        // Assert
        Assert.AreEqual(StatusCertificado.Falha, certificado.Status);
    }

    [TestMethod]
    public void DeveCriar_CertificadoComSolicitacaoInformada()
    {
        // Arrange
        Guid solicitacaoCertificadoId = Guid.NewGuid();

        Certificado certificado = new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            solicitacaoCertificadoId,
            "João da Silva");

        // Act
        Guid resultado = certificado.SolicitacaoCertificadoId;

        // Assert
        Assert.AreEqual(
            solicitacaoCertificadoId,
            resultado);
    }
}