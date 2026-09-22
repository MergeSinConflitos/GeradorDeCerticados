using GeradorDeCertificados.Dominio.Modulos.Certificados;
using Microsoft.Extensions.Configuration;

namespace GeradorDeCertificados.Infraestrutura.Certificados;

public sealed class ArmazenadorDeArquivo : IArmazenadorDeArquivo
{
    private readonly string _pastaBase;

    public ArmazenadorDeArquivo(IConfiguration configuration)
    {
        _pastaBase =
            configuration["Armazenamento:PastaCertificados"]
            ?? Path.Combine(AppContext.BaseDirectory, "Armazenamento", "Certificados");
    }

    public async Task<string> SalvarAsync(
        string nomeArquivo,
        byte[] conteudo,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_pastaBase);

        var caminhoCompleto = Path.Combine(_pastaBase, nomeArquivo);

        await File.WriteAllBytesAsync(caminhoCompleto, conteudo, cancellationToken);

        return caminhoCompleto;
    }

    public async Task<byte[]> LerAsync(
        string caminho,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(caminho))
        {
            throw new FileNotFoundException(
                $"O arquivo \"{caminho}\" não foi encontrado.", caminho);
        }

        return await File.ReadAllBytesAsync(caminho, cancellationToken);
    }
}