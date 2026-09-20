using GeradorDeCertificados.Dominio.Modulos.Certificados;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeradorDeCertificados.Infraestrutura.Modulos.Certificados;

public sealed class SolicitacaoCertificadoConfiguration
    : IEntityTypeConfiguration<SolicitacaoCertificado>
{
    public void Configure(EntityTypeBuilder<SolicitacaoCertificado> builder)
    {
        builder.ToTable("TBSolicitacoesCertificado");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.CursoId)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.CaminhoZip)
            .HasMaxLength(500);

        builder.Property(x => x.DataDeCriacao)
            .IsRequired();

        builder.Property(x => x.DataDeConclusao);

        builder.HasMany(x => x.Certificados)
            .WithOne()
            .HasForeignKey(x => x.SolicitacaoCertificadoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.CursoId);
    }
}