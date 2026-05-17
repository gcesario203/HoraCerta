using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Procedimento;

namespace HoraCerta.Dominio.Agendamento;

public interface IGerenciadorAgendamentos
{
    ICollection<AvaliacaoEntidade> Avaliacoes { get; }

    AgendamentoEntidade IniciarAgendamento(ProcedimentoEntidade procedimento, SlotHorarioEntidade slot);

    void ConfirmarAgendamento(IdEntidade id, IdEntidade proprietarioId);

    void CancelarAgendamento(IdEntidade id, IdEntidade proprietarioId);

    AgendamentoEntidade RemarcarAgendamento(IdEntidade id, SlotHorarioEntidade slot, IdEntidade proprietarioId);

    void AvaliarAgendamento(IdEntidade agendamentoId, int nota, string? comentario, IdEntidade proprietarioId);

    AgendamentoEntidade BuscarAgendamentoPorId(IdEntidade id);

    ICollection<AgendamentoEntidade> BuscarAgendamentos();
}
