using FluentResults;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.Util;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using MediatR;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados;

public sealed record ArquivoDto(string NomeArquivo, byte[] Conteudo);

public sealed record BaixarCertificadoQuery(Guid CursoId) : IRequest<Result<ArquivoDto>>;

public sealed class BaixarCertificadoQueryHandler(
    IRepositorioSolicitacaoCertificado repositorioSolicitacao,
    IArmazenadorDeArquivo armazenador
) : IRequestHandler<BaixarCertificadoQuery, Result<ArquivoDto>>
{
    public async Task<Result<ArquivoDto>> Handle(
        BaixarCertificadoQuery query,
        CancellationToken cancellationToken)
    {
        var solicitacao = await repositorioSolicitacao.SelecionarPorCursoIdAsync(
            query.CursoId, cancellationToken);

        if (solicitacao is null)
            return Result.Fail(ErrosDeCertificado.NaoEncontrado(query.CursoId));

        if (solicitacao.Status != StatusProcessamento.Concluido || solicitacao.CaminhoZip is null)
            return Result.Fail(ErrosDeCertificado.ProcessamentoNaoConcluido());

        var conteudo = await armazenador.LerAsync(solicitacao.CaminhoZip, cancellationToken);

        return Result.Ok(new ArquivoDto($"certificados-{query.CursoId}.zip", conteudo));
    }
}