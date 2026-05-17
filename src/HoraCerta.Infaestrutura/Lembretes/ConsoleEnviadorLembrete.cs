using HoraCerta.Aplicacao.Integracao.Lembretes;
using Microsoft.Extensions.Logging;

namespace HoraCerta.Infaestrutura.Lembretes;

public class ConsoleEnviadorLembrete : IEnviadorLembrete
{
    private readonly ILogger<ConsoleEnviadorLembrete> _logger;

    public ConsoleEnviadorLembrete(ILogger<ConsoleEnviadorLembrete> logger)
    {
        _logger = logger;
    }

    public Task EnviarAsync(LembretePendente lembrete, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Lembrete enviado: agendamento {AgendamentoId}, cliente {ClienteId}, telefone {Telefone}, slot {SlotInicio}",
            lembrete.AgendamentoId,
            lembrete.ClienteId,
            lembrete.TelefoneCliente,
            lembrete.SlotInicio);

        return Task.CompletedTask;
    }
}
