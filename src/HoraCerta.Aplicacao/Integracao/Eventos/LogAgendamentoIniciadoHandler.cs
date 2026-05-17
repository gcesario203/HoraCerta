using HoraCerta.Aplicacao._Shared.Eventos;
using HoraCerta.Dominio.Cliente.Eventos;
using Microsoft.Extensions.Logging;

namespace HoraCerta.Aplicacao.Integracao.Eventos;

public class LogAgendamentoIniciadoHandler : IDomainEventHandler<AgendamentoIniciadoEvent>
{
    private readonly ILogger<LogAgendamentoIniciadoHandler> _logger;

    public LogAgendamentoIniciadoHandler(ILogger<LogAgendamentoIniciadoHandler> logger)
    {
        _logger = logger;
    }

    public void Handle(AgendamentoIniciadoEvent evento)
        => _logger.LogInformation(
            "Agendamento iniciado: {AgendamentoId} cliente {ClienteId}",
            evento.AgendamentoId,
            evento.ClienteId);
}
