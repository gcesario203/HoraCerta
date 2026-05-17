using HoraCerta.Aplicacao._Shared.Eventos;
using HoraCerta.Dominio.Cliente.Eventos;
using Microsoft.Extensions.Logging;

namespace HoraCerta.Aplicacao.Integracao.Eventos;

public class LogAgendamentoRemarcadoHandler : IDomainEventHandler<AgendamentoRemarcadoEvent>
{
    private readonly ILogger<LogAgendamentoRemarcadoHandler> _logger;

    public LogAgendamentoRemarcadoHandler(ILogger<LogAgendamentoRemarcadoHandler> logger)
    {
        _logger = logger;
    }

    public void Handle(AgendamentoRemarcadoEvent evento)
        => _logger.LogInformation(
            "Agendamento remarcado: {AnteriorId} -> {NovoId} cliente {ClienteId}",
            evento.AgendamentoAnteriorId,
            evento.NovoAgendamentoId,
            evento.ClienteId);
}
