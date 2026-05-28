using System.Text.RegularExpressions;
using HoraCerta.Aplicacao.Comunicacao.Ports;

namespace HoraCerta.Infaestrutura.Comunicacao;

public partial class NormalizadorTelefoneE164 : INormalizadorTelefone
{
    public string Normalizar(string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
            return string.Empty;

        var limpo = telefone.Trim();
        if (limpo.StartsWith("whatsapp:", StringComparison.OrdinalIgnoreCase))
            limpo = limpo["whatsapp:".Length..];

        var digitos = Digitos().Replace(limpo, string.Empty);

        if (digitos.StartsWith("55") && digitos.Length is >= 12 and <= 13)
            return $"+{digitos}";

        if (digitos.Length is 10 or 11)
            return $"+55{digitos}";

        if (limpo.StartsWith('+'))
            return $"+{digitos}";

        return digitos.Length > 0 ? $"+{digitos}" : string.Empty;
    }

    public bool SaoEquivalentes(string telefoneA, string telefoneB)
        => Normalizar(telefoneA) == Normalizar(telefoneB);

    [GeneratedRegex(@"\D")]
    private static partial Regex Digitos();
}
