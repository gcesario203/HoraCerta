using HoraCerta.Dominio._Shared.Interfaces;

namespace HoraCerta.Dominio.Cliente.Eventos;

public record AgendamentoConfirmadoEvent(
    string AgendamentoId,
    string ClienteId,
    string ProprietarioId,
    string TelefoneCliente,
    DateTime SlotInicio,
    DateTime OcorreuEm) : IDomainEvent;
