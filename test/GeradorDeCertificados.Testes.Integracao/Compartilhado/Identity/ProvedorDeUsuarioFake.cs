using GeradorDeCertificados.Dominio.Compartilhado.Auth;

namespace GeradorDeCertificados.Testes.Integracao.Compartilhado.Identity;

public sealed class ProvedorDeUsuarioFake(Guid userId) : IProvedorDeUsuario
{
    public Guid? Id => userId;

    public string? Email => null;

    public bool EstaAutenticado => true;

}