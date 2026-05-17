using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao._Shared.Persistencia;
using HoraCerta.Aplicacao._Shared.Sincronizacao;
using HoraCerta.Aplicacao.Agendamento.Commands;
using HoraCerta.Dominio;
using HoraCerta.Dominio._Shared.Enums;
using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Cliente.Servicos.Repositorio;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Agendamento.Handlers;

public class IniciarAgendamentoHandler : ICommandHandler<IniciarAgendamentoCommand, AgendamentoEntidade>
{
    private readonly IProprietarioRepositorio _proprietarioRepositorio;
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IDomainEventDispatcher _dispatcher;

    public IniciarAgendamentoHandler(
        IProprietarioRepositorio proprietarioRepositorio,
        IClienteRepositorio clienteRepositorio,
        IDomainEventDispatcher dispatcher)
    {
        _proprietarioRepositorio = proprietarioRepositorio;
        _clienteRepositorio = clienteRepositorio;
        _dispatcher = dispatcher;
    }

    public AgendamentoEntidade Executar(IniciarAgendamentoCommand command)
    {
        var proprietario = _proprietarioRepositorio.BuscarPorId(command.ProprietarioId)
            ?? throw new OperacaoInvalidaExcessao("Proprietário não encontrado");

        var cliente = _clienteRepositorio.BuscarPorId(command.ClienteId)
            ?? throw new OperacaoInvalidaExcessao("Cliente não encontrado");

        var procedimento = proprietario.GerenciadorProcedimentos.BuscarProcedimentoPorId(command.ProcedimentoId);

        if (procedimento.EstadoEntidade != EstadoEntidade.ATIVO)
            throw new OperacaoInvalidaExcessao("Procedimento inativo não pode ser agendado");

        var slot = proprietario.Horarios.FirstOrDefault(s => s.Id.Valor == command.SlotHorarioId.Valor)
            ?? throw new OperacaoInvalidaExcessao("Slot de horário não encontrado no estabelecimento");

        if (!slot.VerificarDisponibilidade())
            throw new OperacaoInvalidaExcessao("Slot de horário indisponível");

        var agendamento = cliente.GerenciadorAgendamentos.IniciarAgendamento(procedimento, slot);

        SincronizadorSlotsProprietario.AplicarStatusDoAgendamento(proprietario, agendamento);

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
