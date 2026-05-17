using HoraCerta.Api.Contratos;
using HoraCerta.Api.Mapeamento;
using HoraCerta.Aplicacao.Estabelecimento.Commands;
using HoraCerta.Aplicacao.Estabelecimento.Handlers;
using HoraCerta.Aplicacao.Estabelecimento.Queries;

namespace HoraCerta.Api.Endpoints;

public static class ProcedimentosEndpoints
{
    public static RouteGroupBuilder MapProcedimentos(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/proprietarios/{proprietarioId}/procedimentos")
            .WithTags("Procedimentos");

        group.MapGet("/", (string proprietarioId, ListarProcedimentosAtivosHandler handler) =>
        {
            var procedimentos = handler.Executar(new ListarProcedimentosAtivosQuery(RespostaMapeamento.Id(proprietarioId)));
            return Results.Ok(procedimentos.Select(RespostaMapeamento.ParaResposta));
        });

        group.MapPost("/", (
            string proprietarioId,
            CriarProcedimentoRequisicao req,
            CriarProcedimentoHandler handler) =>
        {
            var procedimento = handler.Executar(new CriarProcedimentoCommand(
                RespostaMapeamento.Id(proprietarioId),
                req.Nome,
                req.Valor,
                TimeSpan.FromMinutes(req.TempoEstimadoMinutos)));

            return Results.Created(
                $"/api/proprietarios/{proprietarioId}/procedimentos/{procedimento.Id.Valor}",
                RespostaMapeamento.ParaResposta(procedimento));
        });

        group.MapPost("/{procedimentoId}/inativar", (
            string proprietarioId,
            string procedimentoId,
            InativarProcedimentoHandler handler) =>
        {
            var procedimento = handler.Executar(new InativarProcedimentoCommand(
                RespostaMapeamento.Id(proprietarioId),
                RespostaMapeamento.Id(procedimentoId)));

            return Results.Ok(RespostaMapeamento.ParaResposta(procedimento));
        });

        return group;
    }
}
