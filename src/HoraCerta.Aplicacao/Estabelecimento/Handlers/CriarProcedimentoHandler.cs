using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao._Shared.Persistencia;
using HoraCerta.Aplicacao.Estabelecimento.Commands;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Procedimento;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Estabelecimento.Handlers;

public class CriarProcedimentoHandler : ICommandHandler<CriarProcedimentoCommand, ProcedimentoEntidade>
{
    private readonly IProprietarioRepositorio _proprietarioRepositorio;
    private readonly IDomainEventDispatcher _dispatcher;

    public CriarProcedimentoHandler(
        IProprietarioRepositorio proprietarioRepositorio,
        IDomainEventDispatcher dispatcher)
    {
        _proprietarioRepositorio = proprietarioRepositorio;
        _dispatcher = dispatcher;
    }

    public ProcedimentoEntidade Executar(CriarProcedimentoCommand command)
    {
        var proprietario = _proprietarioRepositorio.BuscarPorId(command.ProprietarioId)
            ?? throw new OperacaoInvalidaExcessao("Proprietário não encontrado");

        proprietario.GerenciadorProcedimentos.CriarProcedimento(
            command.Nome,
            command.Valor,
            command.TempoEstimado);

        var procedimento = proprietario.GerenciadorProcedimentos.RecuperarProcedimentos().Last();

        UnidadeTrabalhoDominio.SalvarEDispararEventos(
            p => _proprietarioRepositorio.Salvar(p),
            proprietario,
            _dispatcher);

        return procedimento;
    }
}
