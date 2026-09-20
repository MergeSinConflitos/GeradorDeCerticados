using GeradorDeCertificados.Dominio.Modulos.Certificados;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeradorDeCertificados.Infraestrutura.Modulos.Certificados;

public sealed class CertificadoConfiguration
    : IEntityTypeConfiguration<Certificado>
{
    public void Configure(EntityTypeBuilder<Certificado> builder)
    {
        builder.ToTable("TBCertificados");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.CursoId)
            .IsRequired();

        builder.Property(x => x.NomeAluno)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.CaminhoArquivo)
            .HasMaxLength(500);

        builder.Property(x => x.DataDeGeracao);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.HasIndex(x => x.CursoId);
    }
}