namespace HoraCerta.Aplicacao.Autenticacao;

public interface ICredencialProprietarioRepositorio
{
    void Salvar(string proprietarioId, string email, string passwordHash);

    CredencialProprietario? BuscarPorEmail(string email);

    bool Existe(string proprietarioId);
}

public record CredencialProprietario(string ProprietarioId, string Email, string PasswordHash);
