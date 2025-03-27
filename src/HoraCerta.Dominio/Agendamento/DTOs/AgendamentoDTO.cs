using HoraCerta.Dominio._Shared;
using HoraCerta.Dominio._Shared.Enums;
using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Procedimento;
using HoraCerta.Dominio.SlotHorario;

namespace HoraCerta.Dominio.Agendamento;

public class AgendamentoDTO : DTOBase
{
    public AgendamentoDTO(string id, DateTime dataCriacao, DateTime? dataAlteracao, EstadoEntidade estadoEntidade,SlotHorarioDTO? slotHorario, EstadoAgendamento estado, AgendamentoDTO? reagendamento, ProcedimentoDTO procedimento) : base(id, dataCriacao, dataAlteracao, estadoEntidade)
    {
        SlotHorario = slotHorario;
        Estado = estado;
        Reagendamento = reagendamento;
        Procedimento = procedimento;
    }

    public SlotHorarioDTO? SlotHorario { get; private set; }

    public EstadoAgendamento Estado { get; private set; }

    public AgendamentoDTO? Reagendamento { get; private set; }

    public ProcedimentoDTO Procedimento { get; private set; }
}
