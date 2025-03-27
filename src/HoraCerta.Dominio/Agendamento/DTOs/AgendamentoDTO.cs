using HoraCerta.Dominio._Shared;
using HoraCerta.Dominio._Shared.Enums;
using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Procedimento;
using HoraCerta.Dominio.SlotHorario;

namespace HoraCerta.Dominio.Agendamento;

public class AgendamentoDTO : DTOBase
{
    public AgendamentoDTO(SlotHorarioDTO slotHorario, EstadoAgendamento estado, AgendamentoDTO? reagendamento, ProcedimentoDTO procedimento
    , ClienteDTO cliente, string id, DateTime dataCriacao, DateTime? dataAlteracao, EstadoEntidade estadoEntidade) : base(id, dataCriacao, dataAlteracao, estadoEntidade)
    {
        SlotHotario = slotHorario;
        Estado = estado;
        Reagendamento = reagendamento;
        Procedimento = procedimento;
        Cliente = cliente;
    }

    public SlotHorarioDTO SlotHotario { get; private set; }

    public EstadoAgendamento Estado { get; private set; }

    public AgendamentoDTO? Reagendamento { get; private set; }

    public ProcedimentoDTO Procedimento { get; private set; }

    public ClienteDTO Cliente { get; private set; }
}
