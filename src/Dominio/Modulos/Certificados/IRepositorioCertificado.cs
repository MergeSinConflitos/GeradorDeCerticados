using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Dominio.Modulos.Certificados;

public interface IRepositorioCertificado : IRepositorio<Certificado>;

public interface IRepositorioSolicitacaoCertificado
    : IRepositorio<SolicitacaoCertificado>
{
    Task<bool> ExisteProcessamentoEmAndamentoAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default);
}