using GeradorDeCertificados.Dominio.Modulos.Cursos;
using GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;

namespace GeradorDeCertificados.Infraestrutura.Modulos.Cursos;

public sealed class RepositorioCursoEmOrm(GeradorDeCertificadoDbContext dbContext
) : RepositorioBaseEmOrm<Curso>(dbContext), IRepositorioCurso;