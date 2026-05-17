using HoraCerta.Api.Autenticacao;
using HoraCerta.Api.Contratos;
using HoraCerta.Api.Mapeamento;
using HoraCerta.Aplicacao.Estabelecimento.Commands;
using HoraCerta.Aplicacao.Estabelecimento.Handlers;
using HoraCerta.Aplicacao.Estabelecimento.Queries;

namespace HoraCerta.Api.Endpoints;

public static class SlotsEndpoints
{
    public static void MapSlots(this IEndpointRouteBuilder app)
    {
        const string route = "/api/proprietarios/{proprietarioId}/slots";

        app.MapGet($"{route}/disponiveis", (string proprietarioId, ListarSlotsDisponiveisHandler handler) =>
        {
            var slots = handler.Executar(new ListarSlotsDisponiveisQuery(RespostaMapeamento.Id(proprietarioId)));
            return Results.Ok(slots.Select(RespostaMapeamento.ParaResposta));
        }).WithTags("Slots");

        var auth = app.MapGroup(route)
            .WithTags("Slots")
            .RequireAuthorization()
            .AddEndpointFilter<ProprietarioAuthorizationFilter>();

        auth.MapPost("/", (
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
    }
}
