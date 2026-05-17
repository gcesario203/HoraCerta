using HoraCerta.Aplicacao._Shared.Eventos;
using HoraCerta.Dominio.Cliente.Eventos;
using Microsoft.Extensions.Logging;

namespace HoraCerta.Aplicacao.Integracao.Eventos;

public class LogAgendamentoCanceladoHandler : IDomainEventHandler<AgendamentoCanceladoEvent>
{
    private readonly ILogger<LogAgendamentoCanceladoHandler> _logger;

    public LogAgendamentoCanceladoHandler(ILogger<LogAgendamentoCanceladoHandler> logger)
    {
        _logger = logger;
    }

    public void Handle(AgendamentoCanceladoEvent evento)
        => _logger.LogInformation(
            "Agendamento cancelado: {AgendamentoId} cliente {ClienteId}",
            evento.AgendamentoId,
            evento.ClienteId);
}
