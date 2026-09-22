using System.IO.Compression;
using GeradorDeCertificados.Dominio.Modulos.Certificados;

namespace GeradorDeCertificados.Infraestrutura.Certificados;

public sealed class EmpacotadorDeArquivosZip : IEmpacotadorDeArquivo
{
    public byte[] Empacotar(IEnumerable<ArquivoParaEmpacotar> arquivos)
    {
        using var memoria = new MemoryStream();

        using (var zip = new ZipArchive(memoria, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var arquivo in arquivos)
            {
                var entrada = zip.CreateEntry(arquivo.NomeArquivo, CompressionLevel.Optimal);

                using var streamEntrada = entrada.Open();
                streamEntrada.Write(arquivo.Conteudo);
            }
        }

        return memoria.ToArray();
    }
}