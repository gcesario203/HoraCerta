using HoraCerta.Dominio;

namespace HoraCerta.Aplicacao.Agendamento.Commands;

public record ConfirmarAgendamentoCommand(
    IdEntidade ClienteId,
    IdEntidade AgendamentoId);
