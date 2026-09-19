using GeradorDeCertificados.Dominio.Modulos.Cursos;
using FluentResults;
using MediatR;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos.Util;
using GeradorDeCertificados.Dominio.Compartilhado.Auth;
using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Aplicacao.Modulos.Cursos;

public sealed record CadastrarCursoCommand(
    string Nome,
    string? Descricao,
    int CargaHoraria,
    DateTime DataConclusao
) : IRequest<Result<Guid>>;

public sealed class CadastrarCursoCommandHandler(IRepositorioCurso repositorioCurso) 
: IRequestHandler<CadastrarCursoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CadastrarCursoCommand command, CancellationToken cancellationToken = default)
    {
       var curso = new Curso(
            Guid.CreateVersion7(),
            command.Nome,
            command.Descricao,
            command.CargaHoraria,
            command.DataConclusao
       );

        var erros = curso.Validar();

        if(erros.Count > 0)
            return Result.Fail(ErrosDeCurso.Validacao(erros));

        try
        {
            await repositorioCurso.CadastrarAsync(curso, cancellationToken);

            return Result.Ok(curso.Id);
        }
        catch (ConflitoDePersistenciaException)
        {
            return Result.Fail(ErrosDeCurso.ConflitoDePersistencia());
        }
    }
}