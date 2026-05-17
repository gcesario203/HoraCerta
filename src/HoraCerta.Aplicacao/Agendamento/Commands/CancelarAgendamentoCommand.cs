using HoraCerta.Dominio;

namespace HoraCerta.Aplicacao.Agendamento.Commands;

public record CancelarAgendamentoCommand(
    IdEntidade ProprietarioId,
    IdEntidade ClienteId,
    IdEntidade AgendamentoId);
