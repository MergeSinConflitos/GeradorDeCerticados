using GeradorDeCertificados.Aplicacao.Modulos.Certificados;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.Mensageria;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace GeradorDeCertificados.Testes.Unitario.Modulos.Certificados.Mensageria;

[TestClass]
public sealed class GerarCertificadosConsumerTests
{
    private Mock<IRepositorioSolicitacaoCertificado> repositorioSolicitacao = null!;
    private Mock<IRepositorioCurso> repositorioCurso = null!;
    private Mock<GeradorPdfCertificado> geradorPdf = null!;
    private Mock<IArmazenadorDeArquivo> armazenador = null!;
    private Mock<IEmpacotadorDeArquivo> empacotador = null!;
    private Mock<ILogger<GerarCertificadosConsumer>> logger = null!;

    private GerarCertificadosConsumer consumer = null!;

    [TestInitialize]
    public void Setup()
    {
        repositorioSolicitacao =
            new Mock<IRepositorioSolicitacaoCertificado>();

        repositorioCurso =
            new Mock<IRepositorioCurso>();

        geradorPdf =
            new Mock<GeradorPdfCertificado>();

        armazenador =
            new Mock<IArmazenadorDeArquivo>();

        empacotador =
            new Mock<IEmpacotadorDeArquivo>();

        logger =
            new Mock<ILogger<GerarCertificadosConsumer>>();

        consumer = new GerarCertificadosConsumer(
            repositorioSolicitacao.Object,
            repositorioCurso.Object,
            geradorPdf.Object,
            armazenador.Object,
            empacotador.Object,
            logger.Object);
    }

    [TestMethod]
    public async Task DeveGerarCertificadosEZip()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();
        Guid solicitacaoId = Guid.CreateVersion7();

        Curso curso = CriarCurso(cursoId);

        SolicitacaoCertificado solicitacao =
            CriarSolicitacao(
                solicitacaoId,
                cursoId);

        byte[] pdfBytes = [1, 2, 3];
        byte[] zipBytes = [4, 5, 6];

        repositorioSolicitacao
            .Setup(x => x.SelecionarPorIdAsync(
                solicitacaoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(solicitacao);

        repositorioCurso
            .Setup(x => x.SelecionarPorIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(curso);

        geradorPdf
            .Setup(x => x.Gerar(
                It.IsAny<DetalhesCertificadoDto>()))
            .Returns(pdfBytes);

        armazenador
            .Setup(x => x.SalvarAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((string nomeArquivo, byte[] _, CancellationToken _) =>
                $"certificados/{nomeArquivo}");

        empacotador
            .Setup(x => x.Empacotar(
                It.IsAny<List<ArquivoParaEmpacotar>>()))
            .Returns(zipBytes);

        // Act
        await consumer.Consume(
            CriarContexto(solicitacaoId));

        // Assert
        Assert.AreEqual(
            StatusProcessamento.Concluido,
            solicitacao.Status);

        Assert.IsFalse(
            string.IsNullOrWhiteSpace(solicitacao.CaminhoZip));

        Assert.IsTrue(
            solicitacao.Certificados.All(
                x => x.Status == StatusCertificado.Gerado));

        geradorPdf.Verify(
            x => x.Gerar(
                It.IsAny<DetalhesCertificadoDto>()),
            Times.Exactly(2));

        armazenador.Verify(
            x => x.SalvarAsync(
                It.Is<string>(nome =>
                    nome.EndsWith(".pdf")),
                It.IsAny<byte[]>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(2));

        empacotador.Verify(
            x => x.Empacotar(
                It.Is<List<ArquivoParaEmpacotar>>(arquivos =>
                    arquivos.Count == 2)),
            Times.Once);

        armazenador.Verify(
            x => x.SalvarAsync(
                It.Is<string>(nome =>
                    nome.EndsWith(".zip")),
                It.IsAny<byte[]>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        repositorioSolicitacao.Verify(
            x => x.EditarAsync(
                solicitacaoId,
                solicitacao,
                It.IsAny<CancellationToken>()),
            Times.Exactly(4));
    }

    [TestMethod]
    public async Task DeveIgnorar_QuandoSolicitacaoNaoForEncontrada()
    {
        // Arrange
        Guid solicitacaoId = Guid.CreateVersion7();

        repositorioSolicitacao
            .Setup(x => x.SelecionarPorIdAsync(
                solicitacaoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((SolicitacaoCertificado?)null);

        // Act
        await consumer.Consume(
            CriarContexto(solicitacaoId));

        // Assert
        repositorioCurso.Verify(
            x => x.SelecionarPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        geradorPdf.Verify(
            x => x.Gerar(
                It.IsAny<DetalhesCertificadoDto>()),
            Times.Never);

        empacotador.Verify(
            x => x.Empacotar(
                It.IsAny<List<ArquivoParaEmpacotar>>()),
            Times.Never);
    }

    [TestMethod]
    public async Task DeveMarcarComoFalha_QuandoCursoNaoForEncontrado()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();
        Guid solicitacaoId = Guid.CreateVersion7();

        SolicitacaoCertificado solicitacao =
            CriarSolicitacao(
                solicitacaoId,
                cursoId);

        repositorioSolicitacao
            .Setup(x => x.SelecionarPorIdAsync(
                solicitacaoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(solicitacao);

        repositorioCurso
            .Setup(x => x.SelecionarPorIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Curso?)null);

        // Act
        await consumer.Consume(
            CriarContexto(solicitacaoId));

        // Assert
        Assert.AreEqual(
            StatusProcessamento.Falha,
            solicitacao.Status);

        repositorioSolicitacao.Verify(
            x => x.EditarAsync(
                solicitacaoId,
                solicitacao,
                It.IsAny<CancellationToken>()),
            Times.Once);

        geradorPdf.Verify(
            x => x.Gerar(
                It.IsAny<DetalhesCertificadoDto>()),
            Times.Never);

        empacotador.Verify(
            x => x.Empacotar(
                It.IsAny<List<ArquivoParaEmpacotar>>()),
            Times.Never);
    }

    [TestMethod]
    public async Task DeveMarcarCertificadoComoFalha_QuandoGeracaoDoPdfFalhar()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();
        Guid solicitacaoId = Guid.CreateVersion7();

        Curso curso = CriarCurso(cursoId);

        SolicitacaoCertificado solicitacao =
            CriarSolicitacao(
                solicitacaoId,
                cursoId);

        repositorioSolicitacao
            .Setup(x => x.SelecionarPorIdAsync(
                solicitacaoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(solicitacao);

        repositorioCurso
            .Setup(x => x.SelecionarPorIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(curso);

        geradorPdf
            .SetupSequence(x => x.Gerar(
                It.IsAny<DetalhesCertificadoDto>()))
            .Throws(new InvalidOperationException())
            .Returns([1, 2, 3]);

        armazenador
            .Setup(x => x.SalvarAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((string nomeArquivo, byte[] _, CancellationToken _) =>
                $"certificados/{nomeArquivo}");

        empacotador
            .Setup(x => x.Empacotar(
                It.IsAny<List<ArquivoParaEmpacotar>>()))
            .Returns([4, 5, 6]);

        // Act
        await consumer.Consume(
            CriarContexto(solicitacaoId));

        // Assert
        Assert.AreEqual(
            StatusCertificado.Falha,
            solicitacao.Certificados[0].Status);

        Assert.AreEqual(
            StatusCertificado.Gerado,
            solicitacao.Certificados[1].Status);

        empacotador.Verify(
            x => x.Empacotar(
                It.Is<List<ArquivoParaEmpacotar>>(
                    arquivos => arquivos.Count == 1)),
            Times.Once);
    }

    [TestMethod]
    public async Task DevePropagarExcecao_QuandoFalharAoSalvarZip()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();
        Guid solicitacaoId = Guid.CreateVersion7();

        Curso curso = CriarCurso(cursoId);

        SolicitacaoCertificado solicitacao =
            CriarSolicitacao(
                solicitacaoId,
                cursoId);

        repositorioSolicitacao
            .Setup(x => x.SelecionarPorIdAsync(
                solicitacaoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(solicitacao);

        repositorioCurso
            .Setup(x => x.SelecionarPorIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(curso);

        geradorPdf
            .Setup(x => x.Gerar(
                It.IsAny<DetalhesCertificadoDto>()))
            .Returns([1, 2, 3]);

        armazenador
    .SetupSequence(x => x.SalvarAsync(
        It.IsAny<string>(),
        It.IsAny<byte[]>(),
        It.IsAny<CancellationToken>()))
    .ReturnsAsync("certificados/certificado-1.pdf")
    .ReturnsAsync("certificados/certificado-2.pdf")
    .ThrowsAsync(new IOException());

        empacotador
     .Setup(x => x.Empacotar(
         It.IsAny<IEnumerable<ArquivoParaEmpacotar>>()))
     .Returns([4, 5, 6]);

        // Act + Assert
        await Assert.ThrowsExactlyAsync<IOException>(
            () => consumer.Consume(
                CriarContexto(solicitacaoId)));

        Assert.AreEqual(
            StatusProcessamento.GerandoZip,
            solicitacao.Status);
    }

    private static Curso CriarCurso(Guid cursoId)
    {
        return new Curso(
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
    }

    private static SolicitacaoCertificado CriarSolicitacao(
        Guid solicitacaoId,
        Guid cursoId)
    {
        List<Certificado> certificados =
        [
            new Certificado(
                Guid.CreateVersion7(),
                cursoId,
                solicitacaoId,
                "Aluno 1"),

            new Certificado(
                Guid.CreateVersion7(),
                cursoId,
                solicitacaoId,
                "Aluno 2")
        ];

        return new SolicitacaoCertificado(
            solicitacaoId,
            cursoId,
            certificados);
    }

    private static ConsumeContext<GerarCertificadosMessage> CriarContexto(
        Guid solicitacaoId)
    {
        Mock<ConsumeContext<GerarCertificadosMessage>> context =
            new();

        context
            .SetupGet(x => x.Message)
            .Returns(
                new GerarCertificadosMessage(
                    solicitacaoId));

        context
            .SetupGet(x => x.CancellationToken)
            .Returns(CancellationToken.None);

        return context.Object;
    }
}