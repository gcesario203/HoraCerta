using HoraCerta.Dominio._Shared.Interfaces;

namespace HoraCerta.Dominio.Cliente.Eventos;

public record AgendamentoConfirmadoEvent(
    string AgendamentoId,
    string ClienteId,
    string TelefoneCliente,
    DateTime OcorreuEm) : IDomainEvent;
