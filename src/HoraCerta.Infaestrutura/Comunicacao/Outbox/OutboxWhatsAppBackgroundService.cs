using System.Text.Json;
using HoraCerta.Aplicacao.Comunicacao;
using HoraCerta.Aplicacao.Comunicacao.Dtos;
using HoraCerta.Aplicacao.Comunicacao.Enums;
using HoraCerta.Aplicacao.Comunicacao.Ports;
using HoraCerta.Aplicacao.Integracao.Lembretes;
using HoraCerta.Infaestrutura.Comunicacao;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HoraCerta.Infaestrutura.Comunicacao.Outbox;

public class OutboxWhatsAppBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly OutboxOptions _options;
    private readonly ILogger<OutboxWhatsAppBackgroundService> _logger;

    public OutboxWhatsAppBackgroundService(
        IServiceScopeFactory scopeFactory,
        IOptions<OutboxOptions> options,
        ILogger<OutboxWhatsAppBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessarAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar outbox WhatsApp");
            }

            await Task.Delay(TimeSpan.FromSeconds(_options.IntervaloProcessamentoSegundos), stoppingToken);
        }
    }

    private async Task ProcessarAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var repositorio = scope.ServiceProvider.GetRequiredService<IMensagemOutboxRepositorio>();
        var enviador = scope.ServiceProvider.GetRequiredService<IEnviadorWhatsApp>();
        var horarioSilencioso = scope.ServiceProvider.GetRequiredService<IHorarioSilenciosoServico>();
        var lembreteRepositorio = scope.ServiceProvider.GetRequiredService<ILembreteRepositorio>();

        var agora = DateTime.UtcNow;
        var pendentes = repositorio.ReservarPendentes(agora, _options.LoteMaximo);

        foreach (var mensagem in pendentes)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            if (horarioSilencioso.EstaEmHorarioSilencioso(agora)
                && mensagem.Tipo != TipoMensagemOutbox.RespostaBot)
            {
                var proximo = horarioSilencioso.ProximoHorarioPermitido(agora);
                repositorio.RegistrarFalha(
                    mensagem.Id,
                    "Horário silencioso",
                    proximo,
                    mensagem.Tentativas);
                continue;
            }

            try
            {
                await enviador.EnviarAsync(mensagem.TelefoneDestino, mensagem.Corpo, cancellationToken);
                repositorio.MarcarEnviado(mensagem.Id);

                if (mensagem.Tipo == TipoMensagemOutbox.Lembrete && !string.IsNullOrWhiteSpace(mensagem.PayloadJson))
                {
                    var payload = JsonSerializer.Deserialize<OutboxPayloadDto>(mensagem.PayloadJson);
                    if (!string.IsNullOrWhiteSpace(payload?.LembreteId))
                        lembreteRepositorio.MarcarEnviado(payload.LembreteId);
                }
            }
            catch (Exception ex)
            {
                var tentativas = mensagem.Tentativas + 1;
                _logger.LogWarning(
                    ex,
                    "Falha ao enviar mensagem outbox {Id} tentativa {Tentativa} para {Telefone}",
                    mensagem.Id,
                    tentativas,
                    TelefoneLog.Sanitizar(mensagem.TelefoneDestino));

                if (tentativas >= _options.MaxTentativas)
                {
                    repositorio.MarcarFalhaDefinitiva(mensagem.Id, ex.Message);
                    continue;
                }

                var backoff = ObterBackoff(tentativas);
                repositorio.RegistrarFalha(
                    mensagem.Id,
                    ex.Message,
                    DateTime.UtcNow.AddSeconds(backoff),
                    tentativas);
            }
        }
    }

    private int ObterBackoff(int tentativas)
    {
        var indice = Math.Min(tentativas - 1, _options.BackoffSegundos.Length - 1);
        return _options.BackoffSegundos[indice];
    }
}
