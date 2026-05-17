using HoraCerta.Api.Autenticacao;
using HoraCerta.Api.Contratos;
using HoraCerta.Api.Mapeamento;
using HoraCerta.Aplicacao.Estabelecimento.Commands;
using HoraCerta.Aplicacao.Estabelecimento.Handlers;
using HoraCerta.Aplicacao.Estabelecimento.Queries;

namespace HoraCerta.Api.Endpoints;

public static class ProcedimentosEndpoints
{
    public static void MapProcedimentos(this IEndpointRouteBuilder app)
    {
        const string route = "/api/proprietarios/{proprietarioId}/procedimentos";

        app.MapGet($"{route}/", (string proprietarioId, ListarProcedimentosAtivosHandler handler) =>
        {
            var procedimentos = handler.Executar(new ListarProcedimentosAtivosQuery(RespostaMapeamento.Id(proprietarioId)));
            return Results.Ok(procedimentos.Select(RespostaMapeamento.ParaResposta));
        }).WithTags("Procedimentos");

        var auth = app.MapGroup(route)
            .WithTags("Procedimentos")
            .RequireAuthorization()
            .AddEndpointFilter<ProprietarioAuthorizationFilter>();

        auth.MapPost("/", (
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

        auth.MapPost("/{procedimentoId}/inativar", (
            string proprietarioId,
            string procedimentoId,
            InativarProcedimentoHandler handler) =>
        {
            var procedimento = handler.Executar(new InativarProcedimentoCommand(
                RespostaMapeamento.Id(proprietarioId),
                RespostaMapeamento.Id(procedimentoId)));

            return Results.Ok(RespostaMapeamento.ParaResposta(procedimento));
        });
    }
}
