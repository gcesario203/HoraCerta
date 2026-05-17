using HoraCerta.Dominio;

namespace HoraCerta.Aplicacao.Agendamento.Commands;

public record RemarcarAgendamentoCommand(
    IdEntidade ProprietarioId,
    IdEntidade ClienteId,
    IdEntidade AgendamentoId,
    IdEntidade NovoSlotHorarioId);
