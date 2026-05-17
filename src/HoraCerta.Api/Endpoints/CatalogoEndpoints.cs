using HoraCerta.Api.Contratos;
using HoraCerta.Aplicacao.Estabelecimento.Dtos;
using HoraCerta.Aplicacao.Estabelecimento.Handlers;
using HoraCerta.Aplicacao.Estabelecimento.Queries;

namespace HoraCerta.Api.Endpoints;

public static class CatalogoEndpoints
{
    public static void MapCatalogo(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/catalogo/estabelecimentos", (
            string? busca,
            ListarEstabelecimentosCatalogoHandler handler) =>
        {
            var itens = handler.Executar(new ListarEstabelecimentosCatalogoQuery(busca));
            return Results.Ok(itens.Select(ParaResposta));
        }).WithTags("Catalogo");
    }

    private static EstabelecimentoCatalogoResposta ParaResposta(EstabelecimentoCatalogoItem item)
        => new(
            item.Id,
            item.Nome,
            item.QuantidadeProcedimentos,
            item.QuantidadeHorariosDisponiveis,
            item.ProximoHorarioInicio,
            item.PrecoMinimo,
            item.PrecoMaximo,
            item.Procedimentos.Select(p => new ProcedimentoCatalogoResposta(
                p.Id,
                p.Nome,
                p.Valor,
                p.TempoEstimadoMinutos)).ToList(),
            item.ProximosHorarios.Select(s => new SlotCatalogoResposta(
                s.Id,
                s.Inicio,
                s.Fim)).ToList());
}
