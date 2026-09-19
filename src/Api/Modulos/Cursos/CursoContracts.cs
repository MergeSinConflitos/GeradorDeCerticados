namespace GeradorDeCertificados.Api.Modulos.Cursos;

public record CadastrarCursoRequest(
    string Nome,
    string? Descricao,
    int CargaHoraria,
    DateTime DataConclusao
);

public record CadastrarCursoResponse(Guid CursoId);

public sealed record CursoResponse(
    Guid Id,
    string Nome,
    string? Descricao,
    int CargaHoraria,
    DateTime DataConclusao
);