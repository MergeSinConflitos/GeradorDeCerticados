using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados;

public sealed record DetalhesCertificadoDto(
    string NomeAluno,
    string NomeCurso,
    int CargaHoraria,
    DateTime DataConclusao
);

public sealed class GeradorPdfCertificado 
{   
    public static void ConfiguracaoQuestPdf()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }
    
    public byte[] Gerar(DetalhesCertificadoDto certificado)
    {
        return CriarDocumento(certificado).GeneratePdf();
    }

    private static IDocument CriarDocumento(DetalhesCertificadoDto certificado)
    {
        return Document.Create(documento =>
        {
            documento.Page(pagina =>
            {
                pagina.Size(PageSizes.A4.Landscape());
                pagina.Margin(2, Unit.Centimetre);
                pagina.PageColor(Colors.White);
                pagina.DefaultTextStyle(style => style.FontSize(14));

                pagina.Content()
                    .AlignCenter()
                    .AlignMiddle()
                    .Column(coluna =>
                    {
                        coluna.Item().AlignCenter()
                            .Text("Certificado de Conclusão")
                            .Bold().FontSize(28).FontColor(Colors.Blue.Darken2);

                        coluna.Item().PaddingTop(30).AlignCenter()
                            .Text(texto =>
                            {
                                texto.Span("Certificamos que ");
                                texto.Span(certificado.NomeAluno).Bold();
                                texto.Span(" concluiu o curso");
                            });

                        coluna.Item().PaddingTop(10).AlignCenter()
                            .Text(certificado.NomeCurso)
                            .SemiBold().FontSize(20);

                        coluna.Item().PaddingTop(10).AlignCenter()
                            .Text($"com carga horária de {certificado.CargaHoraria} horas, " +
                                  $"em {certificado.DataConclusao:dd/MM/yyyy}.");
                    });
            });
        });
    }
}
