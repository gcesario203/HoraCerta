using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao.Estabelecimento.Queries;
using HoraCerta.Dominio;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Estabelecimento.Handlers;

public class ListarSlotsDisponiveisHandler : IQueryHandler<ListarSlotsDisponiveisQuery, ICollection<SlotHorarioEntidade>>
{
    private readonly IProprietarioRepositorio _proprietarioRepositorio;

    public ListarSlotsDisponiveisHandler(IProprietarioRepositorio proprietarioRepositorio)
    {
        _proprietarioRepositorio = proprietarioRepositorio;
    }

    public ICollection<SlotHorarioEntidade> Executar(ListarSlotsDisponiveisQuery query)
    {
        var proprietario = _proprietarioRepositorio.BuscarPorId(query.ProprietarioId)
            ?? throw new OperacaoInvalidaExcessao("Proprietário não encontrado");

        return proprietario.GerenciadorAgenda.BuscarHorariosPorStatus(StatusSlotAgendamento.DISPONIVEL);
    }
}
