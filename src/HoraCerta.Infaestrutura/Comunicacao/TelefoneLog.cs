namespace HoraCerta.Infaestrutura.Comunicacao;

public static class TelefoneLog
{
    public static string Sanitizar(string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone) || telefone.Length < 6)
            return "***";

        return telefone[..4] + new string('*', Math.Max(0, telefone.Length - 7)) + telefone[^3..];
    }
}
