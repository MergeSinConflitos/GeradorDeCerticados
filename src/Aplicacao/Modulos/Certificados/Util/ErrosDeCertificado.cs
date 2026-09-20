using FluentResults;
using GeradorDeCertificados.Aplicacao.Compartilhado;
using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados.Util;

public static class ErrosDeCertificado
{
    public static IEnumerable<Error> Validacao(
        IEnumerable<ErroValidacao> erros)
    {
        return erros.Select(erro => new Error(erro.Mensagem)
            .WithMetadata(nameof(TipoErro), TipoErro.Validacao)
            .WithMetadata("Campo", erro.Campo));
    }

    public static Error NaoEncontrado(Guid id)
    {
        return new Error(
                $"Não foi encontrada a solicitação de certificado {id}.")
            .WithMetadata(
                nameof(TipoErro),
                TipoErro.NaoEncontrado);
    }

    public static Error ProcessamentoEmAndamento()
    {
        return new Error(
                "Já existe um processamento de certificados em andamento para este curso.")
            .WithMetadata(
                nameof(TipoErro),
                TipoErro.Conflito);
    }

    public static Error ConflitoDePersistencia()
    {
        return new Error(
                "Ocorreu um conflito ao persistir a solicitação de certificados.")
            .WithMetadata(
                nameof(TipoErro),
                TipoErro.Conflito);
    }
}