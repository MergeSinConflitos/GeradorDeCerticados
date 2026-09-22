using FluentResults;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using Moq;

namespace GeradorDeCertificados.Testes.Unitario.Modulos.Certificados.Handlers;

[TestClass]
public sealed class BaixarCertificadoQueryHandlerTests
{
    private Mock<IRepositorioSolicitacaoCertificado> repositorioSolicitacao = null!;
    private Mock<IArmazenadorDeArquivo> armazenador = null!;

    private BaixarCertificadoQueryHandler handler = null!;

    [TestInitialize]
    public void Setup()
    {
        repositorioSolicitacao =
            new Mock<IRepositorioSolicitacaoCertificado>();

        armazenador =
            new Mock<IArmazenadorDeArquivo>();

        handler = new BaixarCertificadoQueryHandler(
            repositorioSolicitacao.Object,
            armazenador.Object);
    }

    [TestMethod]
    public async Task DeveBaixarCertificado_QuandoProcessamentoEstiverConcluido()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();

        SolicitacaoCertificado solicitacao =
            CriarSolicitacaoConcluida(cursoId);

        byte[] conteudoZip = [1, 2, 3, 4];

        repositorioSolicitacao
            .Setup(x => x.SelecionarPorCursoIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(solicitacao);

        armazenador
            .Setup(x => x.LerAsync(
                solicitacao.CaminhoZip!,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(conteudoZip);

        BaixarCertificadoQuery query =
            new(cursoId);

        // Act
        Result<ArquivoDto> resultado =
            await handler.Handle(
                query,
                CancellationToken.None);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(resultado.Value);

        Assert.AreEqual(
            $"certificados-{cursoId}.zip",
            resultado.Value.NomeArquivo);

        CollectionAssert.AreEqual(
            conteudoZip,
            resultado.Value.Conteudo);

        armazenador.Verify(
            x => x.LerAsync(
                solicitacao.CaminhoZip!,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task DeveFalhar_QuandoSolicitacaoNaoForEncontrada()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();

        repositorioSolicitacao
            .Setup(x => x.SelecionarPorCursoIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((SolicitacaoCertificado?)null);

        BaixarCertificadoQuery query =
            new(cursoId);

        // Act
        Result<ArquivoDto> resultado =
            await handler.Handle(
                query,
                CancellationToken.None);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        armazenador.Verify(
            x => x.LerAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    public async Task DeveFalhar_QuandoProcessamentoNaoEstiverConcluido()
    {
        // Arrange
        Guid cursoId = Guid.CreateVersion7();

        SolicitacaoCertificado solicitacao =
            CriarSolicitacao(cursoId);

        repositorioSolicitacao
            .Setup(x => x.SelecionarPorCursoIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(solicitacao);

        BaixarCertificadoQuery query =
            new(cursoId);

        // Act
        Result<ArquivoDto> resultado =
            await handler.Handle(
                query,
                CancellationToken.None);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        armazenador.Verify(
            x => x.LerAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static SolicitacaoCertificado CriarSolicitacao(
        Guid cursoId)
    {
        Guid solicitacaoId = Guid.CreateVersion7();

        List<Certificado> certificados =
        [
            new Certificado(
                Guid.CreateVersion7(),
                cursoId,
                solicitacaoId,
                "Aluno 1")
        ];

        return new SolicitacaoCertificado(
            solicitacaoId,
            cursoId,
            certificados);
    }

    private static SolicitacaoCertificado CriarSolicitacaoConcluida(
        Guid cursoId)
    {
        SolicitacaoCertificado solicitacao =
            CriarSolicitacao(cursoId);

        solicitacao.IniciarProcessamento();
        solicitacao.IniciarGeracaoZip();

        solicitacao.Concluir(
            "certificados/certificados.zip");

        return solicitacao;
    }
}