namespace HoraCerta.Api.Contratos;

public record AgendamentoResposta(
    string Id,
    string ClienteId,
    string ProcedimentoId,
    string? SlotHorarioId,
    string Estado,
    string? ReagendamentoId);
