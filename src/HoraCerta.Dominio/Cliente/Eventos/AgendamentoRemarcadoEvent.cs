using HoraCerta.Dominio._Shared.Interfaces;

namespace HoraCerta.Dominio.Cliente.Eventos;

public record AgendamentoRemarcadoEvent(
    string AgendamentoAnteriorId,
    string NovoAgendamentoId,
    string ClienteId,
    string TelefoneCliente,
    DateTime OcorreuEm) : IDomainEvent;
