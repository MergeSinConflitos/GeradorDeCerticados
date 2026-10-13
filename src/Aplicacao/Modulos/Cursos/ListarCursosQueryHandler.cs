
using FluentResults;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos.DTOs;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using MediatR;

namespace GeradorDeCertificados.Aplicacao.Modulos.Cursos;

public record ListarCursosQuery : IRequest<Result<List<CursoDto>>>;

public sealed class ListarCursosQueryHandler(
    IRepositorioCurso repositorioCurso
) : IRequestHandler<ListarCursosQuery, Result<List<CursoDto>>>
{
    public async Task<Result<List<CursoDto>>> Handle(
        ListarCursosQuery query,
        CancellationToken cancellationToken = default)
    {
        List<Curso> cursos = await repositorioCurso.SelecionarTodosAsync(
            cancellationToken
        );

        List<CursoDto> cursosDto = cursos
            .Select(curso => new CursoDto(
                curso.Id,
                curso.Nome,
                curso.Descricao,
                curso.CargaHoraria,
                curso.DataConclusao
            ))
            .ToList();

        return Result.Ok(cursosDto);
    }
}
