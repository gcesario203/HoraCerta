using HoraCerta.Dominio._Shared.Interfaces;

namespace HoraCerta.Dominio.Cliente.Eventos;

public record AgendamentoAvaliadoEvent(
    string AgendamentoId,
    string ClienteId,
    string ProprietarioId,
    int Nota,
    DateTime OcorreuEm) : IDomainEvent;
