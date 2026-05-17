using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Cliente.Eventos;
using HoraCerta.Dominio.Procedimento;

namespace HoraCerta.Dominio.Agendamento;

public class GerenciadorAgendamentos : IGerenciadorAgendamentos
{
    private readonly ClienteEntidade _cliente;

    public ICollection<AgendamentoEntidade> Agendamentos { get; private set; }

    public GerenciadorAgendamentos(ClienteEntidade cliente, ICollection<AgendamentoEntidade>? agendamentos)
    {
        Agendamentos = agendamentos is null || !agendamentos.Any()
            ? new List<AgendamentoEntidade>()
            : agendamentos;

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

    public void ConfirmarAgendamento(IdEntidade id)
    {
        var agendamento = BuscarAgendamentoPorId(id);

        agendamento.AlterarEstado(EstadoAgendamento.CONFIRMADO);

        _cliente.AdicionarEventoDominio(new AgendamentoConfirmadoEvent(
            agendamento.Id.Valor,
            _cliente.Id.Valor,
            _cliente.Telefone,
            DateTime.UtcNow));
    }

    public void CancelarAgendamento(IdEntidade id)
    {
        var agendamento = BuscarAgendamentoPorId(id);

        agendamento.AlterarEstado(EstadoAgendamento.CANCELADO);

        _cliente.AdicionarEventoDominio(new AgendamentoCanceladoEvent(
            agendamento.Id.Valor,
            _cliente.Id.Valor,
            _cliente.Telefone,
            DateTime.UtcNow));
    }

    public AgendamentoEntidade RemarcarAgendamento(IdEntidade id, SlotHorarioEntidade slot)
    {
        var agendamento = BuscarAgendamentoPorId(id);

        var remarcacao = agendamento.AlterarEstado(EstadoAgendamento.REMARCADO, slot);

        Agendamentos.Add(remarcacao);

        _cliente.AdicionarEventoDominio(new AgendamentoRemarcadoEvent(
            agendamento.Id.Valor,
            remarcacao.Id.Valor,
            _cliente.Id.Valor,
            _cliente.Telefone,
            DateTime.UtcNow));

        return remarcacao;
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
