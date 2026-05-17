using HoraCerta.Api.Autenticacao;
using HoraCerta.Api.Contratos;
using HoraCerta.Api.Mapeamento;
using HoraCerta.Aplicacao.Agendamento.Handlers;
using HoraCerta.Aplicacao.Agendamento.Queries;
using HoraCerta.Aplicacao.Estabelecimento.Handlers;
using HoraCerta.Aplicacao.Estabelecimento.Queries;

namespace HoraCerta.Api.Endpoints;

public static class ConsultasEndpoints
{
    public static void MapConsultas(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/clientes/{clienteId}/agendamentos", (
            string clienteId,
            ListarAgendamentosClienteHandler handler) =>
        {
            var agendamentos = handler.Executar(new ListarAgendamentosClienteQuery(RespostaMapeamento.Id(clienteId)));
            return Results.Ok(agendamentos.Select(a => RespostaMapeamento.ParaResposta(a, clienteId)));
        }).WithTags("Consultas");

        var proprietarioGroup = app.MapGroup("/api/proprietarios/{proprietarioId}")
            .WithTags("Consultas")
            .RequireAuthorization()
            .AddEndpointFilter<ProprietarioAuthorizationFilter>();

        proprietarioGroup.MapGet("/agendamentos", (
            string proprietarioId,
            ListarAgendamentosProprietarioHandler handler) =>
        {
            var agendamentos = handler.Executar(
                new ListarAgendamentosProprietarioQuery(RespostaMapeamento.Id(proprietarioId)));

            return Results.Ok(agendamentos.Select(a => new AgendamentoListagemResposta(
                a.AgendamentoId,
                a.ClienteId,
                a.ClienteNome,
                a.ProcedimentoNome,
                a.SlotInicio,
                a.Estado)));
        });

        proprietarioGroup.MapGet("/atendimentos", (
            string proprietarioId,
            ListarAtendimentosHandler handler) =>
        {
            var atendimentos = handler.Executar(
                new ListarAtendimentosQuery(RespostaMapeamento.Id(proprietarioId)));

            return Results.Ok(atendimentos.Select(RespostaMapeamento.ParaResposta));
        });

        proprietarioGroup.MapGet("/agendamentos/{agendamentoId}/avaliacao", (
            string proprietarioId,
            string agendamentoId,
            ObterAvaliacaoAgendamentoHandler handler) =>
        {
            var avaliacao = handler.Executar(new ObterAvaliacaoAgendamentoQuery(
                RespostaMapeamento.Id(proprietarioId),
                RespostaMapeamento.Id(agendamentoId)));

            return avaliacao is null
                ? Results.NotFound()
                : Results.Ok(RespostaMapeamento.ParaResposta(avaliacao));
        });
    }
}
