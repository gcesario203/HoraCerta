using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao.Estabelecimento.Dtos;
using HoraCerta.Aplicacao.Estabelecimento.Queries;
using HoraCerta.Dominio;
using HoraCerta.Dominio._Shared.Enums;
using HoraCerta.Dominio.Proprietario;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;

namespace HoraCerta.Aplicacao.Estabelecimento.Handlers;

public class ListarEstabelecimentosCatalogoHandler
    : IQueryHandler<ListarEstabelecimentosCatalogoQuery, IReadOnlyList<EstabelecimentoCatalogoItem>>
{
    private const int MaxProcedimentosPreview = 4;
    private const int MaxHorariosPreview = 3;

    private readonly IProprietarioRepositorio _proprietarioRepositorio;

    public ListarEstabelecimentosCatalogoHandler(IProprietarioRepositorio proprietarioRepositorio)
    {
        _proprietarioRepositorio = proprietarioRepositorio;
    }

    public IReadOnlyList<EstabelecimentoCatalogoItem> Executar(ListarEstabelecimentosCatalogoQuery query)
    {
        var agora = DateTime.Now;
        var busca = query.Busca?.Trim();

        var itens = _proprietarioRepositorio.ListarTodos()
            .Select(p => Mapear(p, agora))
            .Where(i => i is not null)
            .Cast<EstabelecimentoCatalogoItem>()
            .AsEnumerable();

        if (!string.IsNullOrWhiteSpace(busca))
        {
            itens = itens.Where(i =>
                i.Nome.Contains(busca, StringComparison.OrdinalIgnoreCase));
        }

        return itens
            .OrderBy(i => i.ProximoHorarioInicio ?? DateTime.MaxValue)
            .ThenBy(i => i.Nome, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static EstabelecimentoCatalogoItem? Mapear(ProprietarioEntidade proprietario, DateTime agora)
    {
        if (proprietario.EstadoEntidade != EstadoEntidade.ATIVO)
            return null;

        var procedimentos = proprietario.GerenciadorProcedimentos.RecuperarProcedimentos()
            .Where(p => p.EstadoEntidade == EstadoEntidade.ATIVO)
            .OrderBy(p => p.Nome, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var horarios = proprietario.GerenciadorAgenda
            .BuscarHorariosPorStatus(StatusSlotAgendamento.DISPONIVEL)
            .Where(s => s.Inicio >= agora)
            .OrderBy(s => s.Inicio)
            .ToList();

        if (procedimentos.Count == 0 || horarios.Count == 0)
            return null;

        var procedimentosPreview = procedimentos
            .Take(MaxProcedimentosPreview)
            .Select(p => new ProcedimentoCatalogoResumo(
                p.Id.Valor,
                p.Nome,
                p.Valor,
                (int)p.TempoEstimado.TotalMinutes))
            .ToList();

        var horariosPreview = horarios
            .Take(MaxHorariosPreview)
            .Select(s => new SlotCatalogoResumo(s.Id.Valor, s.Inicio, s.Fim))
            .ToList();

        var valores = procedimentos.Select(p => p.Valor).ToList();

        return new EstabelecimentoCatalogoItem(
            proprietario.Id.Valor,
            proprietario.Nome,
            procedimentos.Count,
            horarios.Count,
            horarios.First().Inicio,
            valores.Min(),
            valores.Max(),
            procedimentosPreview,
            horariosPreview);
    }
}
