using HoraCerta.Api.Autenticacao;
using HoraCerta.Api.Contratos;
using HoraCerta.Api.Mapeamento;
using HoraCerta.Aplicacao.Agendamento.Commands;
using HoraCerta.Aplicacao.Agendamento.Handlers;
using HoraCerta.Aplicacao.Estabelecimento.Commands;
using HoraCerta.Aplicacao.Estabelecimento.Handlers;
using HoraCerta.Dominio.Atendimento;

namespace HoraCerta.Api.Endpoints;

public static class AgendamentosEndpoints
{
    public static void MapAgendamentos(this IEndpointRouteBuilder app)
    {
        const string route = "/api/agendamentos";

        app.MapPost($"{route}/iniciar", (IniciarAgendamentoRequisicao req, IniciarAgendamentoHandler handler) =>
        {
            var agendamento = handler.Executar(new IniciarAgendamentoCommand(
                RespostaMapeamento.Id(req.ProprietarioId),
                RespostaMapeamento.Id(req.ClienteId),
                RespostaMapeamento.Id(req.ProcedimentoId),
                RespostaMapeamento.Id(req.SlotHorarioId)));

            return Results.Created(
                $"/api/agendamentos/{agendamento.Id.Valor}",
                RespostaMapeamento.ParaResposta(agendamento, req.ClienteId));
        }).WithTags("Agendamentos");

        var auth = app.MapGroup(route)
            .WithTags("Agendamentos")
            .RequireAuthorization();

        auth.MapPost("/{agendamentoId}/confirmar", (
            string agendamentoId,
            ConfirmarAgendamentoRequisicao req,
            ConfirmarAgendamentoHandler handler) =>
        {
            var agendamento = handler.Executar(new ConfirmarAgendamentoCommand(
                RespostaMapeamento.Id(req.ProprietarioId),
                RespostaMapeamento.Id(req.ClienteId),
                RespostaMapeamento.Id(agendamentoId)));

            return Results.Ok(RespostaMapeamento.ParaResposta(agendamento, req.ClienteId));
        }).AddEndpointFilter<ProprietarioBodyAuthorizationFilter>();

        auth.MapPost("/{agendamentoId}/cancelar", (
            string agendamentoId,
            CancelarAgendamentoRequisicao req,
            CancelarAgendamentoHandler handler) =>
        {
            var agendamento = handler.Executar(new CancelarAgendamentoCommand(
                RespostaMapeamento.Id(req.ProprietarioId),
                RespostaMapeamento.Id(req.ClienteId),
                RespostaMapeamento.Id(agendamentoId)));

            return Results.Ok(RespostaMapeamento.ParaResposta(agendamento, req.ClienteId));
        }).AddEndpointFilter<ProprietarioBodyAuthorizationFilter>();

        auth.MapPost("/{agendamentoId}/remarcar", (
            string agendamentoId,
            RemarcarAgendamentoRequisicao req,
            RemarcarAgendamentoHandler handler) =>
        {
            var agendamento = handler.Executar(new RemarcarAgendamentoCommand(
                RespostaMapeamento.Id(req.ProprietarioId),
                RespostaMapeamento.Id(req.ClienteId),
                RespostaMapeamento.Id(agendamentoId),
                RespostaMapeamento.Id(req.NovoSlotHorarioId)));

            return Results.Created(
                $"/api/agendamentos/{agendamento.Id.Valor}",
                RespostaMapeamento.ParaResposta(agendamento, req.ClienteId));
        }).AddEndpointFilter<ProprietarioBodyAuthorizationFilter>();

        auth.MapPost("/{agendamentoId}/atendimento", (
            string agendamentoId,
            RegistrarAtendimentoRequisicao req,
            RegistrarAtendimentoHandler handler) =>
        {
            var atendimento = handler.Executar(new RegistrarAtendimentoCommand(
                RespostaMapeamento.Id(req.ProprietarioId),
                RespostaMapeamento.Id(req.ClienteId),
                RespostaMapeamento.Id(agendamentoId),
                req.ValorNegociado));

            return Results.Created(
                $"/api/agendamentos/{agendamentoId}/atendimento/{atendimento.Id.Valor}",
                RespostaMapeamento.ParaResposta(atendimento));
        }).AddEndpointFilter<ProprietarioBodyAuthorizationFilter>();

        app.MapPost("/api/clientes/{clienteId}/agendamentos/{agendamentoId}/avaliar", (
            string clienteId,
            string agendamentoId,
            AvaliarAgendamentoRequisicao req,
            AvaliarAgendamentoHandler handler) =>
        {
            var avaliacao = handler.Executar(new AvaliarAgendamentoCommand(
                RespostaMapeamento.Id(req.ProprietarioId),
                RespostaMapeamento.Id(clienteId),
                RespostaMapeamento.Id(agendamentoId),
                req.Nota,
                req.Comentario));

            return Results.Ok(RespostaMapeamento.ParaResposta(avaliacao));
        }).WithTags("Agendamentos");

        app.MapPatch("/api/proprietarios/{proprietarioId}/atendimentos/{atendimentoId}/estado", (
            string proprietarioId,
            string atendimentoId,
            AlterarEstadoAtendimentoRequisicao req,
            AlterarEstadoAtendimentoHandler handler) =>
        {
            if (!Enum.TryParse<EstadoAtendimento>(req.Estado, true, out var estado))
                return Results.BadRequest(new { mensagem = "Estado inválido" });

            var atendimento = handler.Executar(new AlterarEstadoAtendimentoCommand(
                RespostaMapeamento.Id(proprietarioId),
                RespostaMapeamento.Id(atendimentoId),
                estado));

            return Results.Ok(RespostaMapeamento.ParaResposta(atendimento));
        })
        .WithTags("Agendamentos")
        .RequireAuthorization()
        .AddEndpointFilter<ProprietarioAuthorizationFilter>();
    }
}
