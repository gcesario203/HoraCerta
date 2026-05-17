using HoraCerta.Dominio._Shared.Interfaces;

namespace HoraCerta.Dominio.Cliente.Eventos;

public record AgendamentoIniciadoEvent(
    string AgendamentoId,
    string ClienteId,
    string ProcedimentoId,
    string SlotHorarioId,
    DateTime OcorreuEm) : IDomainEvent;
