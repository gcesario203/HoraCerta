using HoraCerta.Dominio._Shared.Interfaces;

namespace HoraCerta.Dominio.Cliente.Eventos;

public record AgendamentoCanceladoEvent(
    string AgendamentoId,
    string ClienteId,
    string TelefoneCliente,
    DateTime OcorreuEm) : IDomainEvent;
