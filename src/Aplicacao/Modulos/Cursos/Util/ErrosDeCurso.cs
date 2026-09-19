using FluentResults;
using GeradorDeCertificados.Aplicacao.Compartilhado;
using GeradorDeCertificados.Dominio.Compartilhado;


namespace GeradorDeCertificados.Aplicacao.Modulos.Cursos.Util;

public static class ErrosDeCurso
{
    public static IEnumerable<Error> Validacao(IEnumerable<ErroValidacao> erros)
    {
        return erros.Select(erro => new Error(erro.Mensagem)
            .WithMetadata(nameof(TipoErro), TipoErro.Validacao)
            .WithMetadata("Campo", erro.Campo));
    }

    public static Error NaoEncontrado(Guid cursoId)
    {
        return new Error($"Não foi encontrado o curso {cursoId}.")
            .WithMetadata(nameof(TipoErro), TipoErro.NaoEncontrado);
    }

    public static Error ConflitoDePersistencia()
    {
        return new Error("Ocorreu um conflito ao persistir o curso.")
            .WithMetadata(nameof(TipoErro), TipoErro.Conflito);
    }
}