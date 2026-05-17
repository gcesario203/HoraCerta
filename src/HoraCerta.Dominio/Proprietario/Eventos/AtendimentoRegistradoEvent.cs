using HoraCerta.Dominio._Shared.Interfaces;

namespace HoraCerta.Dominio.Proprietario.Eventos;

public record AtendimentoRegistradoEvent(
    string AtendimentoId,
    string AgendamentoId,
    string ProprietarioId,
    string ClienteId,
    DateTime OcorreuEm) : IDomainEvent;
