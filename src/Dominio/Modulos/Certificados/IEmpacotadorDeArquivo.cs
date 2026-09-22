namespace GeradorDeCertificados.Dominio.Modulos.Certificados;

public sealed record ArquivoParaEmpacotar(string NomeArquivo, byte[] Conteudo);

public interface IEmpacotadorDeArquivo
{
    byte[] Empacotar(IEnumerable<ArquivoParaEmpacotar> arquivos);
}