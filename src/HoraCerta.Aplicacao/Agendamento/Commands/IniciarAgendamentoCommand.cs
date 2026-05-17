using HoraCerta.Dominio;

namespace HoraCerta.Aplicacao.Agendamento.Commands;

public record IniciarAgendamentoCommand(
    IdEntidade ProprietarioId,
    IdEntidade ClienteId,
    IdEntidade ProcedimentoId,
    IdEntidade SlotHorarioId);
