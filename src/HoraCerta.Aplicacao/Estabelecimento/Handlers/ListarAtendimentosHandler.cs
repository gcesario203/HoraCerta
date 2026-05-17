using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao.Estabelecimento.Queries;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Atendimento;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Estabelecimento.Handlers;

public class ListarAtendimentosHandler : IQueryHandler<ListarAtendimentosQuery, ICollection<AtendimentoEntidade>>
{
    private readonly IProprietarioRepositorio _proprietarioRepositorio;

    public ListarAtendimentosHandler(IProprietarioRepositorio proprietarioRepositorio)
    {
        _proprietarioRepositorio = proprietarioRepositorio;
    }

    public ICollection<AtendimentoEntidade> Executar(ListarAtendimentosQuery query)
    {
        var proprietario = _proprietarioRepositorio.BuscarPorId(query.ProprietarioId)
            ?? throw new OperacaoInvalidaExcessao("Proprietário não encontrado");

        return proprietario.Atendimentos;
    }
}
