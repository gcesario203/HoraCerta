namespace HoraCerta.Infaestrutura.Persistencia.Registros;

public class WebhookTwilioProcessadoRegistro
{
    public string MessageSid { get; set; } = string.Empty;

    public DateTime ProcessadoEm { get; set; }
}
