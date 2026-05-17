using HoraCerta.Dominio.Agendamento;

namespace HoraCerta.Infaestrutura.Persistencia.Modelos;

public class AgendamentoModelo : PersistenciaModeloBase
{
    public string ProcedimentoId { get; set; } = string.Empty;

    public string? SlotHorarioId { get; set; }

    public string? ReagendamentoId { get; set; }

    public EstadoAgendamento Estado { get; set; }

    /// <summary>Legado — usado apenas na leitura de dados gravados antes da migração por referência.</summary>
    public SlotHorarioModelo? SlotHorario { get; set; }

    public AgendamentoModelo? Reagendamento { get; set; }

    public ProcedimentoModelo? Procedimento { get; set; }
}
