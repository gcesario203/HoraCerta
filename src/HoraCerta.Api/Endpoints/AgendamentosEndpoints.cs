using HoraCerta.Api.Contratos;
using HoraCerta.Api.Mapeamento;
using HoraCerta.Aplicacao.Agendamento.Commands;
using HoraCerta.Aplicacao.Agendamento.Handlers;
using HoraCerta.Aplicacao.Estabelecimento.Commands;
using HoraCerta.Aplicacao.Estabelecimento.Handlers;

namespace HoraCerta.Api.Endpoints;

public static class AgendamentosEndpoints
{
    public static RouteGroupBuilder MapAgendamentos(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/agendamentos").WithTags("Agendamentos");

        group.MapPost("/iniciar", (IniciarAgendamentoRequisicao req, IniciarAgendamentoHandler handler) =>
        {
            var agendamento = handler.Executar(new IniciarAgendamentoCommand(
                RespostaMapeamento.Id(req.ProprietarioId),
                RespostaMapeamento.Id(req.ClienteId),
                RespostaMapeamento.Id(req.ProcedimentoId),
                RespostaMapeamento.Id(req.SlotHorarioId)));

            return Results.Created(
                $"/api/agendamentos/{agendamento.Id.Valor}",
                RespostaMapeamento.ParaResposta(agendamento, req.ClienteId));
        });

        group.MapPost("/{agendamentoId}/confirmar", (
            string agendamentoId,
            ConfirmarAgendamentoRequisicao req,
            ConfirmarAgendamentoHandler handler) =>
        {
            var agendamento = handler.Executar(new ConfirmarAgendamentoCommand(
                RespostaMapeamento.Id(req.ClienteId),
                RespostaMapeamento.Id(agendamentoId)));

            return Results.Ok(RespostaMapeamento.ParaResposta(agendamento, req.ClienteId));
        });

        group.MapPost("/{agendamentoId}/cancelar", (
            string agendamentoId,
            CancelarAgendamentoRequisicao req,
            CancelarAgendamentoHandler handler) =>
        {
            var agendamento = handler.Executar(new CancelarAgendamentoCommand(
                RespostaMapeamento.Id(req.ProprietarioId),
                RespostaMapeamento.Id(req.ClienteId),
                RespostaMapeamento.Id(agendamentoId)));

            return Results.Ok(RespostaMapeamento.ParaResposta(agendamento, req.ClienteId));
        });

        group.MapPost("/{agendamentoId}/remarcar", (
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
        });

        group.MapPost("/{agendamentoId}/atendimento", (
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
        });

        return group;
    }
}
