using GeradorDeCertificados.Api.Compartilhado.Http;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static GeradorDeCertificados.Api.Modulos.Certificados.CertificadoContracts;

namespace GeradorDeCertificados.Api.Modulos.Certificados;

[ApiController]
[Route("cursos/{cursoId:guid}/certificados")]
public sealed class CertificadosController(
    IMediator mediator
) : ControllerBase
{
    [Authorize]
    [HttpPost]
    [ProducesResponseType<SolicitarCertificadosResponse>(StatusCodes.Status202Accepted)]
    public async Task<ActionResult<SolicitarCertificadosResponse>> Solicitar(
        Guid cursoId,
        SolicitarCertificadosRequest request,
        CancellationToken cancellationToken)
    {
        var resultado = await mediator.Send(
            new SolicitarCertificadosCommand(cursoId, request.NomesAlunos),
            cancellationToken);

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        var solicitacao = resultado.Value;

        return Accepted(
            new SolicitarCertificadosResponse(
                solicitacao.Id,
                solicitacao.CursoId,
                solicitacao.DataDeCriacao));
    }

    [Authorize]
    [HttpGet("/cursos/{cursoId:guid}/status")]
    [ProducesResponseType<StatusSolicitacaoResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<StatusSolicitacaoResponse>> ObterStatus(
        Guid cursoId,
        CancellationToken cancellationToken)
    {
        var resultado = await mediator.Send(
            new ObterStatusSolicitacaoQuery(cursoId),
            cancellationToken);

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return Ok(new StatusSolicitacaoResponse(cursoId, resultado.Value));
    }

    [Authorize]
    [HttpGet]
    [ProducesResponseType<List<CertificadoResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CertificadoResponse>>> Listar(
        Guid cursoId,
        CancellationToken cancellationToken)
    {
        var resultado = await mediator.Send(
            new ListarCertificadosQuery(cursoId),
            cancellationToken);

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        List<CertificadoResponse> response = resultado.Value
            .Select(certificado => new CertificadoResponse(
                certificado.Id,
                certificado.CursoId,
                certificado.SolicitacaoCertificadoId,
                certificado.NomeAluno,
                certificado.CaminhoArquivo,
                certificado.DataDeGeracao,
                certificado.Status))
            .ToList();

        return Ok(response);
    }

    [Authorize]
    [HttpGet("download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Download(
        Guid cursoId,
        CancellationToken cancellationToken)
    {
        var resultado = await mediator.Send(
            new BaixarCertificadoQuery(cursoId),
            cancellationToken);

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        var arquivo = resultado.Value;

        return File(arquivo.Conteudo, "application/zip", arquivo.NomeArquivo);
    }
}