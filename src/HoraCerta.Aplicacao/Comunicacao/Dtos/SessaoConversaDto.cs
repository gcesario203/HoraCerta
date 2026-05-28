using HoraCerta.Aplicacao.Comunicacao.Enums;

namespace HoraCerta.Aplicacao.Comunicacao.Dtos;

public class SessaoConversaDto
{
    public string Telefone { get; set; } = string.Empty;

    public string ProprietarioId { get; set; } = string.Empty;

    public PassoFluxoBot Passo { get; set; } = PassoFluxoBot.ResolverEstabelecimento;

    public string? ClienteId { get; set; }

    public string? ProcedimentoId { get; set; }

    public string? SlotHorarioId { get; set; }

    public string? NomePendente { get; set; }

    public DateTime AtualizadoEm { get; set; }

    public DateTime ExpiraEm { get; set; }
}
