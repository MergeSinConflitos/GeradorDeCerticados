using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Dominio.Modulos.Certificados;

public sealed class Certificado : EntidadeBase<Certificado>
{
    public Guid CursoId { get; set; }

    public Guid SolicitacaoCertificadoId { get; set; }

    public string NomeAluno { get; set; } = string.Empty;

    public string? CaminhoArquivo { get; set; }

    public DateOnly? DataDeGeracao { get; set; }

    public StatusCertificado Status { get; set; } = StatusCertificado.Pendente;

    public Certificado(
        Guid id,
        Guid cursoId,
        Guid solicitacaoCertificadoId,
        string nomeAluno)
    {
        Id = id;
        CursoId = cursoId;
        SolicitacaoCertificadoId = solicitacaoCertificadoId;
        NomeAluno = nomeAluno;
        Status = StatusCertificado.Pendente;
    }

    private Certificado()
    {
    }

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (string.IsNullOrWhiteSpace(NomeAluno))
        {
            erros.Add(
                new ErroValidacao(
                    nameof(NomeAluno),
                    "O nome do aluno é obrigatório."));
        }
        else if (NomeAluno.Length > 200)
        {
            erros.Add(
                new ErroValidacao(
                    nameof(NomeAluno),
                    "O nome do aluno deve ter no máximo 200 caracteres."));
        }

        if (CursoId == Guid.Empty)
        {
            erros.Add(
                new ErroValidacao(
                    nameof(CursoId),
                    "O curso é obrigatório."));
        }

        return erros;
    }

    public void MarcarComoGerado(
        string caminhoArquivo,
        DateOnly dataDeGeracao)
    {
        CaminhoArquivo = caminhoArquivo;
        DataDeGeracao = dataDeGeracao;
        Status = StatusCertificado.Gerado;
    }

    public void MarcarComoFalha()
    {
        Status = StatusCertificado.Falha;
    }

    public override void Atualizar(Certificado entidadeAtualizada)
    {
        NomeAluno = entidadeAtualizada.NomeAluno;
        CursoId = entidadeAtualizada.CursoId;
    }
}
