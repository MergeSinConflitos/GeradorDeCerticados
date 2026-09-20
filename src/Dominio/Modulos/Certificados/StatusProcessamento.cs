namespace GeradorDeCertificados.Dominio.Modulos.Certificados;

public enum StatusProcessamento
{
    Pendente,
    GerandoCertificados,
    GerandoZip,
    Concluido,
    Falha
}