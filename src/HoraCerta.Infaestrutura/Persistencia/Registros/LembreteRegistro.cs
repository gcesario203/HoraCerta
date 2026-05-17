namespace HoraCerta.Infaestrutura.Persistencia.Registros;

public class LembreteRegistro
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string ProprietarioId { get; set; } = string.Empty;

    public string ClienteId { get; set; } = string.Empty;

    public string AgendamentoId { get; set; } = string.Empty;

    public string TelefoneCliente { get; set; } = string.Empty;

    public DateTime SlotInicio { get; set; }

    public DateTime EnviarEm { get; set; }

    public string Status { get; set; } = "Pendente";

    public DateTime? EnviadoEm { get; set; }
}
