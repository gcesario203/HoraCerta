using HoraCerta.Dominio._Shared;
using HoraCerta.Dominio;
using HoraCerta.Dominio._Shared.Enums;

namespace HoraCerta.Dominio.SlotHorario;

public class SlotHorarioDTO : DTOBase
{
    public SlotHorarioDTO(string id, DateTime dataCriacao, DateTime? dataAlteracao, EstadoEntidade estadoEntidade, DateTime inicio, DateTime? fim, StatusSlotAgendamento statusSlotAgendamento) : base(id, dataCriacao, dataAlteracao, estadoEntidade)
    {
        Inicio = inicio;
        Fim = fim;
        Status = statusSlotAgendamento;
    }

    public DateTime Inicio { get; private set; }

    public DateTime? Fim { get; private set; }

    public StatusSlotAgendamento Status { get; private set; }
}
