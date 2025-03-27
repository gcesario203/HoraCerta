using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Atendimento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoraCerta.Dominio.Agenda;

public interface IGerenciadorAgenda
{
    ICollection<SlotHorarioEntidade> BuscarHorariosPorStatus(StatusSlotAgendamento status);

    void CriarHorarioDisponivel(DateTime inicioDoHorario);

    AgendamentoEntidade CriarAtendimento(AgendamentoEntidade agendamento, decimal? valorNegociado = null);

    AtendimentoEntidade BuscarAtendimentoPorHorario(IdEntidade idHorario);

    AtendimentoEntidade BuscarAtendimentoPorId(IdEntidade idAtendimento);
    AtendimentoEntidade BuscarAtendimentoPorAgendamento(IdEntidade idAgendamento);

    void AlterarStatusAtendimento(EstadoAtendimento estado, IdEntidade idAtendimento);
}
