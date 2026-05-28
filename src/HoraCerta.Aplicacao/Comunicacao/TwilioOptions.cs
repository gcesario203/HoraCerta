namespace HoraCerta.Aplicacao.Comunicacao;

public class TwilioOptions
{
    public const string Secao = "Twilio";

    public bool Enabled { get; set; }

    public string AccountSid { get; set; } = string.Empty;

    public string AuthToken { get; set; } = string.Empty;

    public string WhatsAppFrom { get; set; } = string.Empty;

    public string WebhookBaseUrl { get; set; } = string.Empty;

    public int SessaoExpiracaoHoras { get; set; } = 24;

    public bool ValidarAssinaturaWebhook { get; set; } = true;
}
