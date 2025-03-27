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
        if(horarios == null || !horarios.Any())
            Horarios = new List<SlotHorarioEntidade>();
        else
            Horarios = horarios;

        if (atendimentos == null || !atendimentos.Any())
            Atendimentos = new List<AtendimentoEntidade>();
        else
            Atendimentos = atendimentos;
    }
}
