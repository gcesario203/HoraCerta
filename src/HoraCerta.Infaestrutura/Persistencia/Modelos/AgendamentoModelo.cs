using HoraCerta.Dominio.Agendamento;

namespace HoraCerta.Infaestrutura.Persistencia.Modelos;

public class AgendamentoModelo : PersistenciaModeloBase
{
    public string ProcedimentoId { get; set; } = string.Empty;

    public string? SlotHorarioId { get; set; }

    /// <summary>Snapshot para listagem sem reidratar o calendário do estabelecimento.</summary>
    public string? ProcedimentoNome { get; set; }

    /// <summary>Snapshot do início do slot reservado.</summary>
    public DateTime? SlotInicio { get; set; }

    public string? ReagendamentoId { get; set; }

    public EstadoAgendamento Estado { get; set; }

    /// <summary>Legado — usado apenas na leitura de dados gravados antes da migração por referência.</summary>
    public SlotHorarioModelo? SlotHorario { get; set; }

    public AgendamentoModelo? Reagendamento { get; set; }

    public ProcedimentoModelo? Procedimento { get; set; }
}
