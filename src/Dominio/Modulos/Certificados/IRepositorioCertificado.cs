using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Dominio.Modulos.Certificados;

public interface IRepositorioCertificado : IRepositorio<Certificado>
{
    Task<List<Certificado>> ListarPorCursoIdAsync(
      Guid cursoId,
      CancellationToken cancellationToken = default);
}

public interface IRepositorioSolicitacaoCertificado
    : IRepositorio<SolicitacaoCertificado>
{
    Task<bool> ExisteProcessamentoEmAndamentoAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default);

    Task<SolicitacaoCertificado?> SelecionarPorCursoIdAsync(
    Guid cursoId,
    CancellationToken cancellationToken = default);
}