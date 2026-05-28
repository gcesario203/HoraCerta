namespace HoraCerta.Aplicacao.Comunicacao;

public class OutboxOptions
{
    public const string Secao = "Outbox";

    public int IntervaloProcessamentoSegundos { get; set; } = 5;

    public int MaxTentativas { get; set; } = 5;

    public int[] BackoffSegundos { get; set; } = [30, 60, 300, 900, 3600];

    public int LoteMaximo { get; set; } = 20;
}
