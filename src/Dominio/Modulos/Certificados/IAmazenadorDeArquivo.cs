namespace GeradorDeCertificados.Dominio.Modulos.Certificados;

public interface IArmazenadorDeArquivo
{
    Task<string> SalvarAsync(
        string nomeArquivo,
        byte[] conteudo,
        CancellationToken cancellationToken = default);

    Task<byte[]> LerAsync(
        string caminho,
        CancellationToken cancellationToken = default);
}