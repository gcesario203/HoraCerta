using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Atendimento;
using HoraCerta.Dominio.Proprietario.Eventos;
using HoraCerta.Dominio.Proprietario;

namespace HoraCerta.Dominio.Agenda;

public class GerenciadorAgenda : IGerenciadorAgenda
{
    private readonly ProprietarioEntidade _proprietario;

    public GerenciadorAgenda(ProprietarioEntidade proprietario)
    {
        _proprietario = proprietario;
    }

    public void CriarHorarioDisponivel(DateTime inicioDoHorario)
    {
        var novoSlot = new SlotHorarioEntidade(inicioDoHorario);

        if (BuscarHorariosPorStatus(StatusSlotAgendamento.DISPONIVEL).Any(novoSlot.ConflitaCom))
            throw new OperacaoInvalidaExcessao("Horário já preenchido");

        _proprietario.Horarios.Add(novoSlot);

        _proprietario.AdicionarEventoDominio(new SlotHorarioDisponibilizadoEvent(
            _proprietario.Id.Valor,
            novoSlot.Id.Valor,
            novoSlot.Inicio,
            DateTime.UtcNow));
    }

    public AgendamentoEntidade CriarAtendimento(AgendamentoEntidade agendamento, IdEntidade clienteId, decimal? valorNegociado = null)
    {
        if (agendamento is null || agendamento.EstadoAtual() != EstadoAgendamento.CONFIRMADO)
            throw new OperacaoInvalidaExcessao("Não é possível criar atendimento a partir de um agendamento inválido");

        agendamento = ValidarConflitosDeHorario(agendamento);

        if (agendamento.EstadoAtual() == EstadoAgendamento.PENDENTE)
            return agendamento;

        agendamento.AlterarEstado(EstadoAgendamento.FINALIZADO);
        agendamento.SlotHorario!.AlterarStatus(StatusSlotAgendamento.CONFIRMADO);

        var novoAtendimento = new AtendimentoEntidade(agendamento, valorNegociado ?? agendamento.Procedimento.Valor);
        _proprietario.Atendimentos.Add(novoAtendimento);
        _proprietario.Horarios.Add(agendamento.SlotHorario!);

        _proprietario.AdicionarEventoDominio(new AtendimentoRegistradoEvent(
            novoAtendimento.Id.Valor,
            agendamento.Id.Valor,
            _proprietario.Id.Valor,
            clienteId.Valor,
            DateTime.UtcNow));

        return agendamento;
    }

    private AgendamentoEntidade ValidarConflitosDeHorario(AgendamentoEntidade agendamento)
    {
        if (BuscarHorariosPorStatus(StatusSlotAgendamento.CONFIRMADO)
            .Any(x => agendamento.SlotHorario != null && agendamento.SlotHorario.ConflitaCom(x)))
        {
            throw new OperacaoInvalidaExcessao("Agendamento com horário que coincide com outros");
        }

        if (_proprietario.Atendimentos.Any(x => x.EstadoAtual() == EstadoAtendimento.PENDENTE
            && x.Origem.SlotHorario != null
            && x.Origem.SlotHorario!.ConflitaCom(agendamento.SlotHorario!)))
        {
            throw new OperacaoInvalidaExcessao("Agendamento com horário que coincide com outros");
        }

        var slotDeTempoConflitante = BuscarHorariosPorStatus(StatusSlotAgendamento.DISPONIVEL)
            .FirstOrDefault(x => agendamento.SlotHorario!.ConflitaCom(x));

        if (slotDeTempoConflitante != null)
            return Reagendar(agendamento, slotDeTempoConflitante);

        return agendamento;
    }

    private AgendamentoEntidade Reagendar(AgendamentoEntidade agendamento, SlotHorarioEntidade slotConflitante)
    {
        _proprietario.Horarios.Remove(slotConflitante);

        return agendamento.AlterarEstado(EstadoAgendamento.REMARCADO, new SlotHorarioEntidade(slotConflitante.Inicio, agendamento.Procedimento.TempoEstimado));
    }

    public void AlterarStatusAtendimento(EstadoAtendimento estado, IdEntidade idAtendimento)
    {
        var atendimento = BuscarAtendimentoPorId(idAtendimento);

        atendimento.AlterarEstado(estado);
    }

    public AtendimentoEntidade BuscarAtendimentoPorHorario(IdEntidade idHorario)
    {
        var atendimento = _proprietario.Atendimentos.FirstOrDefault(x => x.Origem.SlotHorario?.Id.Valor == idHorario.Valor);

        if (atendimento is null)
            throw new OperacaoInvalidaExcessao("Atendimento não encontrado");

        return atendimento;
    }

    public ICollection<SlotHorarioEntidade> BuscarHorariosPorStatus(StatusSlotAgendamento status)
        => _proprietario.Horarios.Where(x => x.Status == status).ToList();

    public AtendimentoEntidade BuscarAtendimentoPorId(IdEntidade idAtendimento)
    {
        var atendimento = _proprietario.Atendimentos.FirstOrDefault(x => x.Id.Valor == idAtendimento.Valor);

        if (atendimento is null)
            throw new OperacaoInvalidaExcessao("Atendimento não encontrado");

        return atendimento;
    }

    public AtendimentoEntidade BuscarAtendimentoPorAgendamento(IdEntidade idAgendamento)
    {
        var atendimento = _proprietario.Atendimentos.FirstOrDefault(x => x.Origem.Id.Valor == idAgendamento.Valor);

        if (atendimento is null)
            throw new OperacaoInvalidaExcessao("Atendimento não encontrado");

        return atendimento;
    }
}
