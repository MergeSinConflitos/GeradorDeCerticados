using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;


namespace GeradorDeCertificados.Infraestrutura.Modulos.Certificados;

public sealed class RepositorioCertificadoEmOrm(
    GeradorDeCertificadoDbContext dbContext
) : RepositorioBaseEmOrm<Certificado>(dbContext), IRepositorioCertificado;
