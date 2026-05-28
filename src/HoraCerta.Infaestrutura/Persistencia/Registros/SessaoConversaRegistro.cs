namespace HoraCerta.Infaestrutura.Persistencia.Registros;

public class SessaoConversaRegistro
{
    public string Telefone { get; set; } = string.Empty;

    public string ProprietarioId { get; set; } = string.Empty;

    public string Passo { get; set; } = string.Empty;

    public string? ClienteId { get; set; }

    public string? ProcedimentoId { get; set; }

    public string? SlotHorarioId { get; set; }

    public string? NomePendente { get; set; }

    public DateTime AtualizadoEm { get; set; }

    public DateTime ExpiraEm { get; set; }
}
