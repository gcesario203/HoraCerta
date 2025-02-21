using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Atendimento;
using HoraCerta.Dominio.Proprietario;

namespace HoraCerta.Dominio.Agenda;

public class GerenciadorAgenda : IGerenciadorAgenda
{
    private readonly ProprietarioEntidade _proprietario;

    public AgendaEntidade Agenda { get; private set; }
    public GerenciadorAgenda(ProprietarioEntidade proprietario, ICollection<SlotHorarioEntidade> horarios, ICollection<AtendimentoEntidade> atendimentos)
    {
        _proprietario = proprietario;
        Agenda = new AgendaEntidade(horarios, atendimentos);
    }

    public void CriarHorarioDisponivel(DateTime inicioDoHorario)
    {
        var novoSlot = new SlotHorarioEntidade(inicioDoHorario);

        if (BuscarHorariosPorStatus(StatusSlotAgendamento.DISPONIVEL).Any(novoSlot.ConflitaCom))
            throw new OperacaoInvalidaExcessao("Horário já preenchido");

        Agenda.Horarios.Add(novoSlot);
    }

    public void CriarAtendimento(AgendamentoEntidade agendamento, decimal? valorNegociado = null)
    {
        if (agendamento is null || agendamento.EstadoAtual() != EstadoAgendamento.CONFIRMADO)
            throw new OperacaoInvalidaExcessao("Nao é possivel criar atendimento a partir de um agendamento inválido");

        if (BuscarHorariosPorStatus(StatusSlotAgendamento.CONFIRMADO).Any(agendamento.SlotHorario.ConflitaCom))
            throw new OperacaoInvalidaExcessao("Agendamento com horário que coincide com outros");

        agendamento.AlterarEstado(EstadoAgendamento.FINALIZADO);

        var novoAtendimento = new AtendimentoEntidade(agendamento, valorNegociado ?? agendamento.Procedimento.Valor);

        Agenda.Atendimentos.Add(novoAtendimento);
    }

    public void AlterarStatusAtendimento(EstadoAtendimento estado, IdEntidade idAtendimento)
    {
        var atendimento = BuscarAtendimentoPorId(idAtendimento);

        atendimento.AlterarEstado(estado);
    }

    public AtendimentoEntidade BuscarAtendimentoPorHorario(IdEntidade idHorario)
    {
        var atendimento = Agenda.Atendimentos.FirstOrDefault(x => x.Origem.SlotHorario.Id.Valor == idHorario.Valor);

        if (atendimento is null)
            throw new OperacaoInvalidaExcessao("Atendimento não encontrado");

        return atendimento;
    }

    public ICollection<SlotHorarioEntidade> BuscarHorariosPorStatus(StatusSlotAgendamento status)
        => Agenda.Horarios.Where(x => x.Status == status).ToList();

    public AtendimentoEntidade BuscarAtendimentoPorId(IdEntidade idAtendimento)
    {
        var atendimento = Agenda.Atendimentos.FirstOrDefault(x => x.Id.Valor == idAtendimento.Valor);

        if (atendimento is null)
            throw new OperacaoInvalidaExcessao("Atendimento não encontrado");

        return atendimento;
    }
}
