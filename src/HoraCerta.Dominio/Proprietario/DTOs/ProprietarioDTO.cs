using HoraCerta.Dominio._Shared;
using HoraCerta.Dominio;
using HoraCerta.Dominio._Shared.Enums;
using HoraCerta.Dominio.Agenda;
using HoraCerta.Dominio.Procedimento;

namespace HoraCerta.Dominio.Proprietario;

public class ProprietarioDTO : DTOBase
{
    public ProprietarioDTO(string id, DateTime dataCriacao, DateTime? dataAlteracao, EstadoEntidade estadoEntidade,string nome, AgendaDTO agenda, ICollection<ProcedimentoDTO>? procedimentos) : base(id, dataCriacao, dataAlteracao, estadoEntidade)
    {
        Nome = nome;

        Agenda = agenda;

        Procedimentos = procedimentos ?? new List<ProcedimentoDTO>();
    }

    public AgendaDTO Agenda { get; set; }

    public ICollection<ProcedimentoDTO>? Procedimentos { get; set; }

    public string Nome { get; set; }
}