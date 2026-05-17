using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao._Shared.Persistencia;
using HoraCerta.Aplicacao.Agendamento.Commands;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Cliente.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Agendamento.Handlers;

public class ConfirmarAgendamentoHandler : ICommandHandler<ConfirmarAgendamentoCommand, AgendamentoEntidade>
{
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IDomainEventDispatcher _dispatcher;

    public ConfirmarAgendamentoHandler(
        IClienteRepositorio clienteRepositorio,
        IDomainEventDispatcher dispatcher)
    {
        _clienteRepositorio = clienteRepositorio;
        _dispatcher = dispatcher;
    }

    public AgendamentoEntidade Executar(ConfirmarAgendamentoCommand command)
    {
        var cliente = _clienteRepositorio.BuscarPorId(command.ClienteId)
            ?? throw new OperacaoInvalidaExcessao("Cliente não encontrado");

        cliente.GerenciadorAgendamentos.ConfirmarAgendamento(command.AgendamentoId);

        var agendamento = cliente.GerenciadorAgendamentos.BuscarAgendamentoPorId(command.AgendamentoId);

        UnidadeTrabalhoDominio.SalvarEDispararEventos(
            cliente => _clienteRepositorio.Salvar(cliente),
            cliente,
            _dispatcher);

        return agendamento;
    }
}
