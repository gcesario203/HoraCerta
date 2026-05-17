using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao._Shared.Persistencia;
using HoraCerta.Aplicacao.Agendamento.Commands;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Atendimento;
using HoraCerta.Dominio.Cliente;
using HoraCerta.Dominio.Cliente.Servicos.Repositorio;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Agendamento.Handlers;

public class AvaliarAgendamentoHandler : ICommandHandler<AvaliarAgendamentoCommand, AvaliacaoEntidade>
{
    private readonly IProprietarioRepositorio _proprietarioRepositorio;
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IDomainEventDispatcher _dispatcher;

    public AvaliarAgendamentoHandler(
        IProprietarioRepositorio proprietarioRepositorio,
        IClienteRepositorio clienteRepositorio,
        IDomainEventDispatcher dispatcher)
    {
        _proprietarioRepositorio = proprietarioRepositorio;
        _clienteRepositorio = clienteRepositorio;
        _dispatcher = dispatcher;
    }

    public AvaliacaoEntidade Executar(AvaliarAgendamentoCommand command)
    {
        var proprietario = _proprietarioRepositorio.BuscarPorId(command.ProprietarioId)
            ?? throw new OperacaoInvalidaExcessao("Proprietário não encontrado");

        var cliente = _clienteRepositorio.BuscarPorId(command.ClienteId, proprietario)
            ?? throw new OperacaoInvalidaExcessao("Cliente não encontrado");

        var atendimento = proprietario.GerenciadorAgenda.BuscarAtendimentoPorAgendamento(command.AgendamentoId);

        if (atendimento.EstadoAtual() != EstadoAtendimento.REALIZADO)
            throw new OperacaoInvalidaExcessao("O atendimento deve estar realizado para permitir avaliação");

        cliente.GerenciadorAgendamentos.AvaliarAgendamento(
            command.AgendamentoId,
            command.Nota,
            command.Comentario,
            command.ProprietarioId);

        var avaliacao = cliente.GerenciadorAgendamentos.Avaliacoes.Last();

        UnidadeTrabalhoDominio.SalvarEDispararEventos(
            _clienteRepositorio.Salvar,
            cliente,
            _dispatcher);

        return avaliacao;
    }
}
