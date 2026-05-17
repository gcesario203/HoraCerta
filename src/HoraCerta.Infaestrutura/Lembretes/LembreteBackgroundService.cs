using HoraCerta.Aplicacao.Integracao.Lembretes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HoraCerta.Infaestrutura.Lembretes;

public class LembreteBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<LembreteOptions> _options;
    private readonly ILogger<LembreteBackgroundService> _logger;

    public LembreteBackgroundService(
        IServiceScopeFactory scopeFactory,
        IOptions<LembreteOptions> options,
        ILogger<LembreteBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessarLembretesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar lembretes pendentes");
            }

            await Task.Delay(TimeSpan.FromMinutes(_options.Value.IntervaloMinutos), stoppingToken);
        }
    }

    private async Task ProcessarLembretesAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var repositorio = scope.ServiceProvider.GetRequiredService<ILembreteRepositorio>();
        var enviador = scope.ServiceProvider.GetRequiredService<IEnviadorLembrete>();

        var pendentes = repositorio.BuscarPendentesParaEnvio(DateTime.UtcNow);

        foreach (var lembrete in pendentes)
        {
            await enviador.EnviarAsync(lembrete, cancellationToken);
            repositorio.MarcarEnviado(lembrete.Id);
        }
    }
}
