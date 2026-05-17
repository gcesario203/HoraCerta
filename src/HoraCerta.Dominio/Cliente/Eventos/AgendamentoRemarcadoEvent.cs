using HoraCerta.Dominio._Shared.Interfaces;

namespace HoraCerta.Dominio.Cliente.Eventos;

public record AgendamentoRemarcadoEvent(
    string AgendamentoAnteriorId,
    string NovoAgendamentoId,
    string ClienteId,
    string ProprietarioId,
    string TelefoneCliente,
    DateTime NovoSlotInicio,
    DateTime OcorreuEm) : IDomainEvent;
