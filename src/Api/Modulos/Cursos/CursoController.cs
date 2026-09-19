using GeradorDeCertificados.Api.Compartilhado.Http;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GeradorDeCertificados.Api.Modulos.Cursos;

[ApiController]
[Route("Api/cursos")]
public sealed class CursoController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<CadastrarCursoResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<CadastrarCursoResponse>> Cadastrar(
        CadastrarCursoRequest request,
        CancellationToken cancellationToken)
    {
        var resultado = await mediator.Send(new CadastrarCursoCommand(
            request.Nome,
            request.Descricao,
            request.CargaHoraria,
            request.DataConclusao
        ), cancellationToken);

        if (!resultado.IsSuccess)
            return this.ProblemDetails(resultado);

        var response = new CadastrarCursoResponse(resultado.Value);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { cursoId = resultado.Value },
            response
        );
    }

    [HttpGet("{cursoId:guid}")]
    [ProducesResponseType<CursoResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CursoResponse>> ObterPorId(
        Guid cursoId,
        CancellationToken cancellationToken)
    {
        var resultado = await mediator.Send(
            new ObterCursoPorIdQuery(cursoId),
            cancellationToken
        );

        if (!resultado.IsSuccess)
            return this.ProblemDetails(resultado);

        var curso = resultado.Value;

        return Ok(new CursoResponse(
            curso.Id,
            curso.Nome,
            curso.Descricao,
            curso.CargaHoraria,
            curso.DataConclusao
        ));
    }
}