using HoraCerta.Aplicacao.Comunicacao.Ports;
using HoraCerta.Infaestrutura.Comunicacao;
using Microsoft.Extensions.Logging;

namespace HoraCerta.Infaestrutura.Comunicacao.ProvedorTwilio;

public class ConsoleEnviadorWhatsApp : IEnviadorWhatsApp
{
    private readonly ILogger<ConsoleEnviadorWhatsApp> _logger;

    public ConsoleEnviadorWhatsApp(ILogger<ConsoleEnviadorWhatsApp> logger)
    {
        _logger = logger;
    }

    public Task EnviarAsync(string telefoneDestino, string corpo, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[WhatsApp simulado] Para {Telefone}: {Corpo}",
            TelefoneLog.Sanitizar(telefoneDestino),
            corpo);
        return Task.CompletedTask;
    }
}
