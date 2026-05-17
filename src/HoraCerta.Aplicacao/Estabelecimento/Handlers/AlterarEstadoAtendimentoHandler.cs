using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao._Shared.Persistencia;
using HoraCerta.Aplicacao.Estabelecimento.Commands;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Atendimento;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Estabelecimento.Handlers;

public class AlterarEstadoAtendimentoHandler : ICommandHandler<AlterarEstadoAtendimentoCommand, AtendimentoEntidade>
{
    private readonly IProprietarioRepositorio _proprietarioRepositorio;
    private readonly IDomainEventDispatcher _dispatcher;

    public AlterarEstadoAtendimentoHandler(
        IProprietarioRepositorio proprietarioRepositorio,
        IDomainEventDispatcher dispatcher)
    {
        _proprietarioRepositorio = proprietarioRepositorio;
        _dispatcher = dispatcher;
    }

    public AtendimentoEntidade Executar(AlterarEstadoAtendimentoCommand command)
    {
        var proprietario = _proprietarioRepositorio.BuscarPorId(command.ProprietarioId)
            ?? throw new OperacaoInvalidaExcessao("Proprietário não encontrado");

        proprietario.GerenciadorAgenda.AlterarStatusAtendimento(command.NovoEstado, command.AtendimentoId);

        var atendimento = proprietario.GerenciadorAgenda.BuscarAtendimentoPorId(command.AtendimentoId);

        UnidadeTrabalhoDominio.SalvarEDispararEventos(
            _proprietarioRepositorio.Salvar,
            proprietario,
            _dispatcher);

        return atendimento;
    }
}
