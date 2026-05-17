using HoraCerta.Dominio._Shared;
using HoraCerta.Dominio._Shared.Enums;
using HoraCerta.Dominio.Atendimento;
using HoraCerta.Dominio.Procedimento;
using HoraCerta.Dominio.SlotHorario;

namespace HoraCerta.Dominio.Proprietario;

public class ProprietarioDTO : DTOBase
{
    public ProprietarioDTO(
        string id,
        DateTime dataCriacao,
        DateTime? dataAlteracao,
        EstadoEntidade estadoEntidade,
        string nome,
        ICollection<SlotHorarioDTO>? horarios,
        ICollection<AtendimentoDTO>? atendimentos,
        ICollection<ProcedimentoDTO>? procedimentos) : base(id, dataCriacao, dataAlteracao, estadoEntidade)
    {
        Nome = nome;
        Horarios = horarios ?? new List<SlotHorarioDTO>();
        Atendimentos = atendimentos ?? new List<AtendimentoDTO>();
        Procedimentos = procedimentos ?? new List<ProcedimentoDTO>();
    }

    public string Nome { get; set; }

    public ICollection<SlotHorarioDTO> Horarios { get; set; }

    public ICollection<AtendimentoDTO> Atendimentos { get; set; }

    public ICollection<ProcedimentoDTO> Procedimentos { get; set; }
}
