using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Dominio.Modulos.Certificados;

public sealed class SolicitacaoCertificado : EntidadeBase<SolicitacaoCertificado>
{
    public Guid CursoId { get; set; }

    public StatusProcessamento Status { get; set; }

    public string? CaminhoZip { get; set; }

    public DateOnly DataDeCriacao { get; set; }

    public DateOnly? DataDeConclusao { get; set; }

    public List<Certificado> Certificados { get; set; } = [];

    public SolicitacaoCertificado(
        Guid id,
        Guid cursoId,
        List<Certificado> certificados)
    {
        Id = id;
        CursoId = cursoId;
        Certificados = certificados;
        Status = StatusProcessamento.Pendente;
        DataDeCriacao = DateOnly.FromDateTime(DateTime.UtcNow);
    }

    private SolicitacaoCertificado()
    {
    }

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (CursoId == Guid.Empty)
        {
            erros.Add(
                new ErroValidacao(
                    nameof(CursoId),
                    "O curso é obrigatório."));
        }

        if (Certificados.Count == 0)
        {
            erros.Add(
                new ErroValidacao(
                    nameof(Certificados),
                    "A solicitação deve possuir pelo menos um aluno."));
        }

        return erros;
    }

    public void IniciarProcessamento()
    {
        Status = StatusProcessamento.GerandoCertificados;
    }

    public void IniciarGeracaoZip()
    {
        Status = StatusProcessamento.GerandoZip;
    }

    public void Concluir(string caminhoZip)
    {
        CaminhoZip = caminhoZip;
        DataDeConclusao = DateOnly.FromDateTime(DateTime.UtcNow);
        Status = StatusProcessamento.Concluido;
    }

    public void MarcarComoFalha()
    {
        Status = StatusProcessamento.Falha;
    }

    public override void Atualizar(SolicitacaoCertificado entidadeAtualizada)
    {
        CursoId = entidadeAtualizada.CursoId;
        Status = entidadeAtualizada.Status;
        CaminhoZip = entidadeAtualizada.CaminhoZip;
        DataDeConclusao = entidadeAtualizada.DataDeConclusao;
        Certificados = entidadeAtualizada.Certificados;
    }
}