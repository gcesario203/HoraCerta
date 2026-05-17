using HoraCerta.Api.Contratos;
using HoraCerta.Api.Mapeamento;
using HoraCerta.Aplicacao.Estabelecimento.Commands;
using HoraCerta.Aplicacao.Estabelecimento.Handlers;
using HoraCerta.Aplicacao.Estabelecimento.Queries;

namespace HoraCerta.Api.Endpoints;

public static class SlotsEndpoints
{
    public static RouteGroupBuilder MapSlots(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/proprietarios/{proprietarioId}/slots")
            .WithTags("Slots");

        group.MapGet("/disponiveis", (string proprietarioId, ListarSlotsDisponiveisHandler handler) =>
        {
            var slots = handler.Executar(new ListarSlotsDisponiveisQuery(RespostaMapeamento.Id(proprietarioId)));
            return Results.Ok(slots.Select(RespostaMapeamento.ParaResposta));
        });

        group.MapPost("/", (
            string proprietarioId,
            CriarSlotRequisicao req,
            CriarSlotDisponivelHandler handler) =>
        {
            var slot = handler.Executar(new CriarSlotDisponivelCommand(
                RespostaMapeamento.Id(proprietarioId),
                req.Inicio));

            return Results.Created(
                $"/api/proprietarios/{proprietarioId}/slots/{slot.Id.Valor}",
                RespostaMapeamento.ParaResposta(slot));
        });

        return group;
    }
}
