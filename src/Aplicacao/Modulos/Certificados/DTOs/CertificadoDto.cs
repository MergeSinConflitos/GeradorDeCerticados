using GeradorDeCertificados.Dominio.Modulos.Certificados;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados.DTOs;

public record CertificadoDto(
    Guid Id,
    Guid CursoId,
    Guid SolicitacaoCertificadoId,
    string NomeAluno,
    string? CaminhoArquivo,
    DateOnly? DataDeGeracao,
    StatusCertificado Status
);

public record SolicitacaoCertificadoDto(
    Guid Id,
    Guid CursoId,
    string? CaminhoZip,
    DateOnly DataDeCriacao,
    DateOnly? DataDeConclusao,
    List<CertificadoDto> Certificados
);