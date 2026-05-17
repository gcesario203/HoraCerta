using HoraCerta.Aplicacao._Shared.Eventos;
using HoraCerta.Dominio.Cliente.Eventos;
using Microsoft.Extensions.Logging;

namespace HoraCerta.Aplicacao.Integracao.Eventos;

public class LogAgendamentoConfirmadoHandler : IDomainEventHandler<AgendamentoConfirmadoEvent>
{
    private readonly ILogger<LogAgendamentoConfirmadoHandler> _logger;

    public LogAgendamentoConfirmadoHandler(ILogger<LogAgendamentoConfirmadoHandler> logger)
    {
        _logger = logger;
    }

    public void Handle(AgendamentoConfirmadoEvent evento)
        => _logger.LogInformation(
            "Agendamento confirmado: {AgendamentoId} cliente {ClienteId}",
            evento.AgendamentoId,
            evento.ClienteId);
}
