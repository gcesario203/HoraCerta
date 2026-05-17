namespace HoraCerta.Aplicacao.Autenticacao;

public class JwtOptions
{
    public const string Secao = "Jwt";

    public string Issuer { get; set; } = "HoraCerta";

    public string Audience { get; set; } = "HoraCerta";

    public string Key { get; set; } = "HoraCerta-Dev-Secret-Key-Min-32-Chars!!";

    public int ExpiracaoHoras { get; set; } = 8;
}
