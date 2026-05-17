using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao.Estabelecimento.Queries;
using HoraCerta.Dominio;
using HoraCerta.Dominio._Shared.Enums;
using HoraCerta.Dominio.Procedimento;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Estabelecimento.Handlers;

public class ListarProcedimentosAtivosHandler : IQueryHandler<ListarProcedimentosAtivosQuery, ICollection<ProcedimentoEntidade>>
{
    private readonly IProprietarioRepositorio _proprietarioRepositorio;

    public ListarProcedimentosAtivosHandler(IProprietarioRepositorio proprietarioRepositorio)
    {
        _proprietarioRepositorio = proprietarioRepositorio;
    }

    public ICollection<ProcedimentoEntidade> Executar(ListarProcedimentosAtivosQuery query)
    {
        var proprietario = _proprietarioRepositorio.BuscarPorId(query.ProprietarioId)
            ?? throw new OperacaoInvalidaExcessao("Proprietário não encontrado");

        return proprietario.GerenciadorProcedimentos.RecuperarProcedimentos()
            .Where(p => p.EstadoEntidade == EstadoEntidade.ATIVO)
            .ToList();
    }
}
