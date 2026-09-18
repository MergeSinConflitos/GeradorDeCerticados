using GeradorDeCertificados.Dominio.Modulos.Cursos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeradorDeCertificados.Infraestrutura.Compartilhado.Orm.Config;

public sealed class CursoConfiguration : IEntityTypeConfiguration<Curso>
{
    public void Configure(EntityTypeBuilder<Curso> builder)
    {
        builder.ToTable("TBCursos");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever(); // O Id é gerado pelo próprio domínio, não pelo banco de dados.

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Descricao)
            .HasMaxLength(500);

        builder.Property(c => c.CargaHoraria)
            .IsRequired();

        builder.Property(c => c.DataConclusao)
            .IsRequired();
    }
}