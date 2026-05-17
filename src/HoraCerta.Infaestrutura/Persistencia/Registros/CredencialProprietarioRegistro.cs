namespace HoraCerta.Infaestrutura.Persistencia.Registros;

public class CredencialProprietarioRegistro
{
    public string ProprietarioId { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;
}
