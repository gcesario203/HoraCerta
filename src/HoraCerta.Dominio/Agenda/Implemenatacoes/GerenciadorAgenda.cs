using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Atendimento;
using HoraCerta.Dominio.Proprietario;

namespace HoraCerta.Dominio.Agenda;

public class GerenciadorAgenda : IGerenciadorAgenda
{
    private readonly ProprietarioEntidade _proprietario;

    public AgendaEntidade Agenda { get; private set; }
    public GerenciadorAgenda(ProprietarioEntidade proprietario, ICollection<SlotHorarioEntidade>? horarios, ICollection<AtendimentoEntidade>? atendimentos)
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

    public AgendamentoEntidade CriarAtendimento(AgendamentoEntidade agendamento, decimal? valorNegociado = null)
    {
        if (agendamento is null || agendamento.EstadoAtual() != EstadoAgendamento.CONFIRMADO)
            throw new OperacaoInvalidaExcessao("Não é possível criar atendimento a partir de um agendamento inválido");

        // Validação de conflitos de horários
        agendamento = ValidarConflitosDeHorario(agendamento);

        if(agendamento.EstadoAtual() == EstadoAgendamento.PENDENTE)
            return agendamento;

        // Atualiza o estado do agendamento e o status do horário
        agendamento.AlterarEstado(EstadoAgendamento.FINALIZADO);
        agendamento.SlotHorario!.AlterarStatus(StatusSlotAgendamento.CONFIRMADO);

        // Cria o novo atendimento e adiciona na agenda
        var novoAtendimento = new AtendimentoEntidade(agendamento, valorNegociado ?? agendamento.Procedimento.Valor);
        Agenda.Atendimentos.Add(novoAtendimento);
        Agenda.Horarios.Add(agendamento.SlotHorario!);

        return agendamento;
    }

    private AgendamentoEntidade ValidarConflitosDeHorario(AgendamentoEntidade agendamento)
    {
        // Verifica se o agendamento já tem horário confirmado
        if (BuscarHorariosPorStatus(StatusSlotAgendamento.CONFIRMADO)
            .Any(x => agendamento.SlotHorario != null && agendamento.SlotHorario.ConflitaCom(x)))
        {
            throw new OperacaoInvalidaExcessao("Agendamento com horário que coincide com outros");
        }

        // Verifica conflitos com atendimentos pendentes
        if (Agenda.Atendimentos.Any(x => x.EstadoAtual() == EstadoAtendimento.PENDENTE
            && x.Origem.SlotHorario != null
            && x.Origem.SlotHorario!.ConflitaCom(agendamento.SlotHorario!)))
        {
            throw new OperacaoInvalidaExcessao("Agendamento com horário que coincide com outros");
        }

        // Verifica se o horário do novo agendamento conflita com algum horário disponível
        var slotDeTempoConflitante = BuscarHorariosPorStatus(StatusSlotAgendamento.DISPONIVEL)
            .FirstOrDefault(x => agendamento.SlotHorario!.ConflitaCom(x));

        if (slotDeTempoConflitante != null)
        {
            // Caso haja conflito, reagenda o agendamento
            return Reagendar(agendamento, slotDeTempoConflitante);
        }

        return agendamento;
    }

    private AgendamentoEntidade Reagendar(AgendamentoEntidade agendamento, SlotHorarioEntidade slotConflitante)
    {
        Agenda.Horarios.Remove(slotConflitante);

        return agendamento.AlterarEstado(EstadoAgendamento.REMARCADO, new SlotHorarioEntidade(slotConflitante.Inicio, agendamento.Procedimento.TempoEstimado));
    }

    public void AlterarStatusAtendimento(EstadoAtendimento estado, IdEntidade idAtendimento)
    {
        var atendimento = BuscarAtendimentoPorId(idAtendimento);

        atendimento.AlterarEstado(estado);
    }

    public AtendimentoEntidade BuscarAtendimentoPorHorario(IdEntidade idHorario)
    {
        var atendimento = Agenda.Atendimentos.FirstOrDefault(x => x.Origem.SlotHorario?.Id.Valor == idHorario.Valor);

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

    public AtendimentoEntidade BuscarAtendimentoPorAgendamento(IdEntidade idAgendamento)
    {
        var atendimento = Agenda.Atendimentos.FirstOrDefault(x => x.Origem.Id.Valor == idAgendamento.Valor);

        if (atendimento is null)
            throw new OperacaoInvalidaExcessao("Atendimento não encontrado");

        return atendimento;
    }
}
