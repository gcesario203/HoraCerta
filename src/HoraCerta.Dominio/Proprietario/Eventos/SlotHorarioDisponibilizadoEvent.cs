using HoraCerta.Dominio._Shared.Interfaces;

namespace HoraCerta.Dominio.Proprietario.Eventos;

public record SlotHorarioDisponibilizadoEvent(
    string ProprietarioId,
    string SlotHorarioId,
    DateTime Inicio,
    DateTime OcorreuEm) : IDomainEvent;
