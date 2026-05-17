using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao._Shared.Persistencia;
using HoraCerta.Aplicacao.Estabelecimento.Commands;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Agendamento;
using HoraCerta.Dominio.Atendimento;
using HoraCerta.Dominio.Cliente.Servicos.Repositorio;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Estabelecimento.Handlers;

public class RegistrarAtendimentoHandler : ICommandHandler<RegistrarAtendimentoCommand, AtendimentoEntidade>
{
    private readonly IProprietarioRepositorio _proprietarioRepositorio;
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IDomainEventDispatcher _dispatcher;

    public RegistrarAtendimentoHandler(
        IProprietarioRepositorio proprietarioRepositorio,
        IClienteRepositorio clienteRepositorio,
        IDomainEventDispatcher dispatcher)
    {
        _proprietarioRepositorio = proprietarioRepositorio;
        _clienteRepositorio = clienteRepositorio;
        _dispatcher = dispatcher;
    }

    public AtendimentoEntidade Executar(RegistrarAtendimentoCommand command)
    {
        var proprietario = _proprietarioRepositorio.BuscarPorId(command.ProprietarioId)
            ?? throw new OperacaoInvalidaExcessao("Proprietário não encontrado");

        var cliente = _clienteRepositorio.BuscarPorId(command.ClienteId, proprietario)
            ?? throw new OperacaoInvalidaExcessao("Cliente não encontrado");

        var agendamento = cliente.GerenciadorAgendamentos.BuscarAgendamentoPorId(command.AgendamentoId);

        if (agendamento.EstadoAtual() != EstadoAgendamento.CONFIRMADO)
            throw new OperacaoInvalidaExcessao("Somente agendamentos confirmados podem gerar atendimento");

        proprietario.GerenciadorAgenda.CriarAtendimento(agendamento, command.ClienteId, command.ValorNegociado);

        var atendimento = proprietario.GerenciadorAgenda.BuscarAtendimentoPorAgendamento(command.AgendamentoId);

        UnidadeTrabalhoDominio.SalvarEDispararEventos(
            () =>
            {
                _proprietarioRepositorio.Salvar(proprietario);
                _clienteRepositorio.Salvar(cliente);
            },
            [proprietario, cliente],
            _dispatcher);

        return atendimento;
    }
}
