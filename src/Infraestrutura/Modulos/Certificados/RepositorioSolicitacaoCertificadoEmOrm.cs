using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeCertificados.Infraestrutura.Modulos.Certificados;

public sealed class RepositorioSolicitacaoCertificadoEmOrm(
    GeradorDeCertificadoDbContext dbContext
) : RepositorioBaseEmOrm<SolicitacaoCertificado>(dbContext), IRepositorioSolicitacaoCertificado
{
    public override async Task<SolicitacaoCertificado?> SelecionarPorIdAsync(
        Guid idSelecionado,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<SolicitacaoCertificado>()
            .Include(x => x.Certificados)
            .SingleOrDefaultAsync(x => x.Id == idSelecionado, cancellationToken);
    }

    public async Task<bool> ExisteProcessamentoEmAndamentoAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<SolicitacaoCertificado>()
            .AnyAsync(
                x =>
                    x.CursoId == cursoId &&
                    x.Status != StatusProcessamento.Concluido &&
                    x.Status != StatusProcessamento.Falha,
                cancellationToken);
    }

    public async Task<SolicitacaoCertificado?> SelecionarPorCursoIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext
            .Set<SolicitacaoCertificado>()
            .Include(x => x.Certificados)
            .FirstOrDefaultAsync(
                x => x.CursoId == cursoId,
                cancellationToken);
    }
}