using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao._Shared.Persistencia;
using HoraCerta.Aplicacao.Agendamento.Commands;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Cliente.Servicos.Repositorio;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Agendamento.Handlers;

public class RemarcarAgendamentoHandler : ICommandHandler<RemarcarAgendamentoCommand, AgendamentoEntidade>
{
    private readonly IProprietarioRepositorio _proprietarioRepositorio;
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IDomainEventDispatcher _dispatcher;

    public RemarcarAgendamentoHandler(
        IProprietarioRepositorio proprietarioRepositorio,
        IClienteRepositorio clienteRepositorio,
        IDomainEventDispatcher dispatcher)
    {
        _proprietarioRepositorio = proprietarioRepositorio;
        _clienteRepositorio = clienteRepositorio;
        _dispatcher = dispatcher;
    }

    public AgendamentoEntidade Executar(RemarcarAgendamentoCommand command)
    {
        var proprietario = _proprietarioRepositorio.BuscarPorId(command.ProprietarioId)
            ?? throw new OperacaoInvalidaExcessao("Proprietário não encontrado");

        var cliente = _clienteRepositorio.BuscarPorId(command.ClienteId, proprietario)
            ?? throw new OperacaoInvalidaExcessao("Cliente não encontrado");

        var novoSlot = proprietario.Horarios.FirstOrDefault(s => s.Id.Valor == command.NovoSlotHorarioId.Valor)
            ?? throw new OperacaoInvalidaExcessao("Slot de horário não encontrado no estabelecimento");

        if (!novoSlot.VerificarDisponibilidade())
            throw new OperacaoInvalidaExcessao("Slot de horário indisponível");

        var remarcado = cliente.GerenciadorAgendamentos.RemarcarAgendamento(
            command.AgendamentoId,
            novoSlot,
            command.ProprietarioId);

        UnidadeTrabalhoDominio.SalvarEDispararEventos(
            () =>
            {
                _clienteRepositorio.Salvar(cliente);
                _proprietarioRepositorio.Salvar(proprietario);
            },
            [cliente, proprietario],
            _dispatcher);

        return remarcado;
    }
}
