namespace HoraCerta.Api.Contratos;

public record SlotHorarioResposta(
    string Id,
    DateTime Inicio,
    DateTime? Fim,
    string Status);
