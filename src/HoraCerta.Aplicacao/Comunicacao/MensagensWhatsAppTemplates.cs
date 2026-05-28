namespace HoraCerta.Aplicacao.Comunicacao;

public static class MensagensWhatsAppTemplates
{
    public static string Confirmacao(DateTime slotInicioUtc)
        => $"Seu agendamento foi confirmado para {FormatarDataHora(slotInicioUtc)}. Até lá!";

    public static string Cancelamento(DateTime slotInicioUtc)
        => $"Seu agendamento de {FormatarDataHora(slotInicioUtc)} foi cancelado. Entre em contato com o estabelecimento se precisar remarcar.";

    public static string Remarcacao(DateTime novoSlotInicioUtc)
        => $"Seu agendamento foi remarcado para {FormatarDataHora(novoSlotInicioUtc)}.";

    public static string Lembrete(DateTime slotInicioUtc)
        => $"Lembrete: você tem agendamento em {FormatarDataHora(slotInicioUtc)}.";

    public static string OptOutConfirmado()
        => "Você não receberá mais mensagens automáticas por aqui. Para voltar a agendar, use o link do estabelecimento.";

    private static string FormatarDataHora(DateTime utc)
    {
        var local = utc.ToLocalTime();
        return local.ToString("dd/MM/yyyy 'às' HH:mm");
    }
}
