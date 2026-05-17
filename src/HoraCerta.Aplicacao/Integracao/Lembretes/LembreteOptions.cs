namespace HoraCerta.Aplicacao.Integracao.Lembretes;

public class LembreteOptions
{
    public const string Secao = "Lembretes";

    public int HorasAntecedencia { get; set; } = 24;

    public int IntervaloMinutos { get; set; } = 15;
}
