using HoraCerta.Dominio;

namespace HoraCerta.Aplicacao.Agendamento.Commands;

public record AvaliarAgendamentoCommand(
    IdEntidade ProprietarioId,
    IdEntidade ClienteId,
    IdEntidade AgendamentoId,
    int Nota,
    string? Comentario);
