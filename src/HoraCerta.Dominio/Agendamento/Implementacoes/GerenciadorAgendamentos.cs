using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Cliente.Eventos;
using HoraCerta.Dominio.Procedimento;

namespace HoraCerta.Dominio.Agendamento;

public class GerenciadorAgendamentos : IGerenciadorAgendamentos
{
    private readonly ClienteEntidade _cliente;

    public ICollection<AgendamentoEntidade> Agendamentos { get; private set; }

    public ICollection<AvaliacaoEntidade> Avaliacoes { get; private set; }

    public GerenciadorAgendamentos(
        ClienteEntidade cliente,
        ICollection<AgendamentoEntidade>? agendamentos,
        ICollection<AvaliacaoEntidade>? avaliacoes = null)
    {
        Agendamentos = agendamentos is null || !agendamentos.Any()
            ? new List<AgendamentoEntidade>()
            : agendamentos;

        Avaliacoes = avaliacoes is null || !avaliacoes.Any()
            ? new List<AvaliacaoEntidade>()
            : avaliacoes;

        _cliente = cliente;
    }

    public AgendamentoEntidade IniciarAgendamento(ProcedimentoEntidade procedimento, SlotHorarioEntidade slot)
    {
        var agendamento = new AgendamentoEntidade(slot, procedimento);

        Agendamentos.Add(agendamento);

        _cliente.AdicionarEventoDominio(new AgendamentoIniciadoEvent(
            agendamento.Id.Valor,
            _cliente.Id.Valor,
            procedimento.Id.Valor,
            slot.Id.Valor,
            DateTime.UtcNow));

        return agendamento;
    }

    public void ConfirmarAgendamento(IdEntidade id, IdEntidade proprietarioId)
    {
        var agendamento = BuscarAgendamentoPorId(id);

        agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);

        var slotInicio = agendamento.SlotHorario?.Inicio
            ?? throw new OperacaoInvalidaExcessao("Agendamento sem slot associado");

        _cliente.AdicionarEventoDominio(new AgendamentoConfirmadoEvent(
            agendamento.Id.Valor,
            _cliente.Id.Valor,
            proprietarioId.Valor,
            _cliente.Telefone,
            slotInicio,
            DateTime.UtcNow));
    }

    public void CancelarAgendamento(IdEntidade id, IdEntidade proprietarioId)
    {
        var agendamento = BuscarAgendamentoPorId(id);

        var slotInicio = agendamento.SlotHorario?.Inicio ?? DateTime.UtcNow;

        agendamento.AlterarEstado(EstadoAgendamento.CANCELADO);

        _cliente.AdicionarEventoDominio(new AgendamentoCanceladoEvent(
            agendamento.Id.Valor,
            _cliente.Id.Valor,
            proprietarioId.Valor,
            _cliente.Telefone,
            slotInicio,
            DateTime.UtcNow));
    }

    public AgendamentoEntidade RemarcarAgendamento(IdEntidade id, SlotHorarioEntidade slot, IdEntidade proprietarioId)
    {
        var agendamento = BuscarAgendamentoPorId(id);

        var remarcacao = agendamento.AlterarEstado(EstadoAgendamento.REMARCADO, slot);

        Agendamentos.Add(remarcacao);

        _cliente.AdicionarEventoDominio(new AgendamentoRemarcadoEvent(
            agendamento.Id.Valor,
            remarcacao.Id.Valor,
            _cliente.Id.Valor,
            proprietarioId.Valor,
            _cliente.Telefone,
            slot.Inicio,
            DateTime.UtcNow));

        return remarcacao;
    }

    public void AvaliarAgendamento(IdEntidade agendamentoId, int nota, string? comentario, IdEntidade proprietarioId)
    {
        var agendamento = BuscarAgendamentoPorId(agendamentoId);

        if (agendamento.EstadoAtual() is not EstadoAgendamento.CONFIRMADO and not EstadoAgendamento.FINALIZADO)
            throw new OperacaoInvalidaExcessao("Somente agendamentos confirmados ou finalizados podem ser avaliados");

        if (Avaliacoes.Any(x => x.AgendamentoId.Valor == agendamentoId.Valor))
            throw new OperacaoInvalidaExcessao("Agendamento já foi avaliado");

        var avaliacao = new AvaliacaoEntidade(agendamentoId, proprietarioId, nota, comentario);
        Avaliacoes.Add(avaliacao);

        _cliente.AdicionarEventoDominio(new AgendamentoAvaliadoEvent(
            agendamentoId.Valor,
            _cliente.Id.Valor,
            proprietarioId.Valor,
            nota,
            DateTime.UtcNow));
    }

    public AgendamentoEntidade BuscarAgendamentoPorId(IdEntidade id)
    {
        var agendamento = Agendamentos.FirstOrDefault(x => x.Id.Valor == id.Valor);

        if (agendamento is null)
            throw new OperacaoInvalidaExcessao("Agendamento não encontrado");

        return agendamento;
    }

    public ICollection<AgendamentoEntidade> BuscarAgendamentos()
        => Agendamentos;
}
