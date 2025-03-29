using HoraCerta.Dominio._Shared;
using HoraCerta.Dominio._Shared.Enums;
using HoraCerta.Dominio.Atendimento;
using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Procedimento;
using HoraCerta.Dominio.SlotHorario;

namespace HoraCerta.Dominio.Agenda;

public class AgendaDTO : DTOBase
{
    public AgendaDTO(string id, DateTime dataCriacao, DateTime? dataAlteracao, EstadoEntidade estadoEntidade, ICollection<SlotHorarioDTO>? horarios, ICollection<AtendimentoDTO>? atendimentos) : base(id, dataCriacao, dataAlteracao, estadoEntidade)
    {
        Horarios = horarios ?? new List<SlotHorarioDTO>();
        Atendimentos = atendimentos ?? new List<AtendimentoDTO>();
    }

    public ICollection<SlotHorarioDTO> Horarios { get; private set; }

    public ICollection<AtendimentoDTO> Atendimentos { get; private set; }
}