using HoraCerta.Dominio._Shared.Enums;
using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Atendimento;
using HoraCerta.Dominio.Proprietario;

namespace HoraCerta.Dominio.Agenda;

public class AgendaEntidade : EntidadeBase<AgendaEntidade>
{
    public ICollection<SlotHorarioEntidade> Horarios { get; private set; }

    public ICollection<AtendimentoEntidade> Atendimentos { get; private set; }

    public AgendaEntidade(ICollection<SlotHorarioEntidade>? horarios, ICollection<AtendimentoEntidade>? atendimentos)
    {
        if (horarios == null || !horarios.Any())
            Horarios = new List<SlotHorarioEntidade>();
        else
            Horarios = horarios;

        if (atendimentos == null || !atendimentos.Any())
            Atendimentos = new List<AtendimentoEntidade>();
        else
            Atendimentos = atendimentos;
    }

    private AgendaEntidade(string id, DateTime dataCriacao, DateTime? dataAlteracao, EstadoEntidade estadoEntidade, ICollection<SlotHorarioEntidade>? horarios, ICollection<AtendimentoEntidade>? atendimentos)
    : base(id, dataCriacao, dataAlteracao, estadoEntidade)
    {
        if (horarios == null || !horarios.Any())
            Horarios = new List<SlotHorarioEntidade>();
        else
            Horarios = horarios;

        if (atendimentos == null || !atendimentos.Any())
            Atendimentos = new List<AtendimentoEntidade>();
        else
            Atendimentos = atendimentos;
    }

    public static AgendaDTO ParaDTO(AgendaEntidade agenda)
        => new AgendaDTO(agenda.Id.Valor, agenda.DataCriacao, agenda.DataAlteracao, agenda.EstadoEntidade, agenda.Horarios?.Select(SlotHorarioEntidade.ParaDTO)?.ToList(), agenda.Atendimentos?.Select(AtendimentoEntidade.ParaDTO)?.ToList());

    public static AgendaEntidade ParaEntidade(AgendaDTO agenda)
        => new AgendaEntidade(agenda.Id, agenda.DataCriacao, agenda.DataAlteracao, agenda.EstadoEntidade, agenda.Horarios?.Select(SlotHorarioEntidade.ParaEntidade)?.ToList(), agenda.Atendimentos?.Select(AtendimentoEntidade.ParaEntidade)?.ToList());
}
