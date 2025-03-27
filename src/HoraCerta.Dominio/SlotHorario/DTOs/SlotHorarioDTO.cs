using HoraCerta.Dominio._Shared;
using HoraCerta.Dominio;
using HoraCerta.Dominio._Shared.Enums;

namespace HoraCerta.Dominio.SlotHorario;

public class SlotHorarioDTO : DTOBase
{
    public SlotHorarioDTO(DateTime inicio, DateTime? fim, StatusSlotAgendamento statusSlotAgendamento, string id, DateTime dataCriacao, DateTime? dataAlteracao, EstadoEntidade estadoEntidade) : base(id, dataCriacao, dataAlteracao, estadoEntidade)
    {
        Inicio = inicio;
        Fim = fim;
        Status = statusSlotAgendamento;
    }

    public DateTime Inicio { get; private set; }

    public DateTime? Fim { get; private set; }

    public StatusSlotAgendamento Status { get; private set; }
}
