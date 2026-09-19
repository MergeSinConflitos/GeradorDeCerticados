using FluentResults;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos.DTOs;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos.Util;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using MediatR;

namespace GeradorDeCertificados.Aplicacao.Modulos.Cursos;

public record ObterCursoPorIdQuery(Guid CursoId): IRequest<Result<CursoDto>>;

public sealed class ObterCursoPorIdQueryHandler(IRepositorioCurso repositorioCurso)
: IRequestHandler<ObterCursoPorIdQuery, Result<CursoDto>>
{
    public async Task<Result<CursoDto>> Handle(
        ObterCursoPorIdQuery query, 
        CancellationToken cancellationToken = default)
    {
        Curso? curso = await repositorioCurso.SelecionarPorIdAsync(
            query.CursoId,
            cancellationToken
        );

        if(curso is null)
            return Result.Fail(ErrosDeCurso.NaoEncontrado(query.CursoId));

        return Result.Ok(new CursoDto(
            curso.Id,
            curso.Nome,
            curso.Descricao,
            curso.CargaHoraria,
            curso.DataConclusao
        ));
    }
}