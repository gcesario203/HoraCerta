namespace HoraCerta.Aplicacao.Comunicacao;

public class WhatsAppOptions
{
    public const string Secao = "WhatsApp";

    public string HorarioSilenciosoInicio { get; set; } = "22:00";

    public string HorarioSilenciosoFim { get; set; } = "08:00";

    public string FusoHorario { get; set; } = "America/Sao_Paulo";
}
