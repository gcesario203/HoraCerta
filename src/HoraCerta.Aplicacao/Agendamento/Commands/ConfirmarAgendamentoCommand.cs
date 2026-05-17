using HoraCerta.Dominio;

namespace HoraCerta.Aplicacao.Agendamento.Commands;

public record ConfirmarAgendamentoCommand(
    IdEntidade ProprietarioId,
    IdEntidade ClienteId,
    IdEntidade AgendamentoId);
