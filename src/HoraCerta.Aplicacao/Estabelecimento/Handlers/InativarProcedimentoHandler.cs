using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao._Shared.Persistencia;
using HoraCerta.Aplicacao.Estabelecimento.Commands;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Procedimento;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Estabelecimento.Handlers;

public class InativarProcedimentoHandler : ICommandHandler<InativarProcedimentoCommand, ProcedimentoEntidade>
{
    private readonly IProprietarioRepositorio _proprietarioRepositorio;
    private readonly IDomainEventDispatcher _dispatcher;

    public InativarProcedimentoHandler(
        IProprietarioRepositorio proprietarioRepositorio,
        IDomainEventDispatcher dispatcher)
    {
        _proprietarioRepositorio = proprietarioRepositorio;
        _dispatcher = dispatcher;
    }

    public ProcedimentoEntidade Executar(InativarProcedimentoCommand command)
    {
        var proprietario = _proprietarioRepositorio.BuscarPorId(command.ProprietarioId)
            ?? throw new OperacaoInvalidaExcessao("Proprietário não encontrado");

        var procedimento = proprietario.GerenciadorProcedimentos.BuscarProcedimentoPorId(command.ProcedimentoId);

        proprietario.GerenciadorProcedimentos.InativarProcedimento(procedimento);

        UnidadeTrabalhoDominio.SalvarEDispararEventos(
            p => _proprietarioRepositorio.Salvar(p),
            proprietario,
            _dispatcher);

        return procedimento;
    }
}
