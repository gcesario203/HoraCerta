using HoraCerta.Dominio.Agendamento;

namespace HoraCerta.Infaestrutura.Persistencia.Modelos;

public class AgendamentoModelo : PersistenciaModeloBase
{
    public SlotHorarioModelo? SlotHorario { get; set; }

    public EstadoAgendamento Estado { get; set; }

    public AgendamentoModelo? Reagendamento { get; set; }

    public ProcedimentoModelo Procedimento { get; set; } = null!;
}
