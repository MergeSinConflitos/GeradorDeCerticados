using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Dominio.Modulos.Cursos;

public class Curso : EntidadeBase<Curso>
{

    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int CargaHoraria { get; set; }
    public DateTime DataConclusao { get; set; }

    private Curso() { }

    public Curso(Guid id, string nome, string? descricao, int cargaHoraria, DateTime dataConclusao)
    {
        Id = id;
        Nome = nome;
        Descricao = descricao;
        CargaHoraria = cargaHoraria;
        DataConclusao = dataConclusao;
    }

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (string.IsNullOrWhiteSpace(Nome) || Nome.Length > 200)
        {
            erros.Add(new ErroValidacao(nameof(Nome), "O nome do curso é obrigatório e deve ter no máximo 200 caracteres."));
        }

        if (Descricao?.Length > 500)
        {
            erros.Add(new ErroValidacao(nameof(Descricao), "A descrição do curso é opcional mas deve ter no máximo 500 caracteres."));
        }

        if (CargaHoraria <= 0)
        {
            erros.Add(new ErroValidacao(nameof(CargaHoraria), "A carga horária do curso é obrigatória e deve ser um valor positivo."));
        }

        if (DataConclusao == DateTime.MinValue)
        {
            erros.Add(new ErroValidacao(nameof(DataConclusao), "A data de conclusão do curso é obrigatória."));
        }

        return erros;
    }

    public override void Atualizar(Curso entidadeAtualizada)
    {
        Nome = entidadeAtualizada.Nome;
        Descricao = entidadeAtualizada.Descricao;
        CargaHoraria = entidadeAtualizada.CargaHoraria;
        DataConclusao = entidadeAtualizada.DataConclusao;
    }
}

/*
O curso deve possuir nome (máx. 200 caracteres).
● O curso deve possuir uma descrição opcional (máx. 500 caracteres).
● O curso deve possuir uma carga horária (inteiro maior que zero).
● O curso deve possuir uma data de conclusão (obrigatória).
● Um curso inexistente não pode receber uma solicitação de geração de certificados.
*/