using GeradorDeCertificados.Dominio.Modulos.Certificados;

namespace GeradorDeCertificados.Api.Modulos.Certificados;

public sealed class CertificadoContracts
{
    public sealed record SolicitarCertificadosRequest(
        List<string> NomesAlunos
    );

    public sealed record SolicitarCertificadosResponse(
        Guid Id,
        Guid CursoId,
        DateOnly DataDeCriacao
    );

    public sealed record CertificadoResponse(
        Guid Id,
        Guid CursoId,
        Guid SolicitacaoCertificadoId,
        string NomeAluno,
        string? CaminhoArquivo,
        DateOnly? DataDeGeracao,
        StatusCertificado Status
    );

    public sealed record StatusSolicitacaoResponse(
        Guid CursoId,
        StatusProcessamento Status
    );
}