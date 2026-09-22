using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;


namespace GeradorDeCertificados.Infraestrutura.Modulos.Certificados;

public sealed class RepositorioCertificadoEmOrm(
    GeradorDeCertificadoDbContext dbContext
) : RepositorioBaseEmOrm<Certificado>(dbContext), IRepositorioCertificado
{
    public async Task<List<Certificado>> ListarPorCursoIdAsync(
     Guid cursoId,
     CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Certificado>()
            .Where(x => x.CursoId == cursoId)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }
}