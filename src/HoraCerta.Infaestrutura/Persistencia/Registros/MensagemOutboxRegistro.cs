namespace HoraCerta.Infaestrutura.Persistencia.Registros;

public class MensagemOutboxRegistro
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Tipo { get; set; } = string.Empty;

    public string TelefoneDestino { get; set; } = string.Empty;

    public string ProprietarioId { get; set; } = string.Empty;

    public string Corpo { get; set; } = string.Empty;

    public string? PayloadJson { get; set; }

    public string? IdempotencyKey { get; set; }

    public string Status { get; set; } = "Pendente";

    public int Tentativas { get; set; }

    public DateTime ProximaTentativaEm { get; set; }

    public DateTime CriadoEm { get; set; }

    public DateTime? EnviadoEm { get; set; }

    public string? UltimoErro { get; set; }
}
