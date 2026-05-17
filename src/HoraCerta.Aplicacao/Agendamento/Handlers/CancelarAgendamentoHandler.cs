using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao._Shared.Persistencia;
using HoraCerta.Aplicacao._Shared.Sincronizacao;
using HoraCerta.Aplicacao.Agendamento.Commands;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Cliente.Servicos.Repositorio;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Agendamento.Handlers;

public class CancelarAgendamentoHandler : ICommandHandler<CancelarAgendamentoCommand, AgendamentoEntidade>
{
    private readonly IProprietarioRepositorio _proprietarioRepositorio;
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IDomainEventDispatcher _dispatcher;

    public CancelarAgendamentoHandler(
        IProprietarioRepositorio proprietarioRepositorio,
        IClienteRepositorio clienteRepositorio,
        IDomainEventDispatcher dispatcher)
    {
        _proprietarioRepositorio = proprietarioRepositorio;
        _clienteRepositorio = clienteRepositorio;
        _dispatcher = dispatcher;
    }

    public AgendamentoEntidade Executar(CancelarAgendamentoCommand command)
    {
        var proprietario = _proprietarioRepositorio.BuscarPorId(command.ProprietarioId)
            ?? throw new OperacaoInvalidaExcessao("Proprietário não encontrado");

        var cliente = _clienteRepositorio.BuscarPorId(command.ClienteId)
            ?? throw new OperacaoInvalidaExcessao("Cliente não encontrado");

        var agendamento = cliente.GerenciadorAgendamentos.BuscarAgendamentoPorId(command.AgendamentoId);
        var slotId = agendamento.SlotHorario?.Id;

        cliente.GerenciadorAgendamentos.CancelarAgendamento(command.AgendamentoId);

        if (slotId is not null)
            SincronizadorSlotsProprietario.LiberarSlot(proprietario, slotId);

        UnidadeTrabalhoDominio.SalvarEDispararEventos(
            () =>
            {
                _clienteRepositorio.Salvar(cliente);
                _proprietarioRepositorio.Salvar(proprietario);
            },
            [cliente, proprietario],
            _dispatcher);

        return agendamento;
    }
}
