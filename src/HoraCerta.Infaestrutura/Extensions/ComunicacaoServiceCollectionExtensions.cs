using HoraCerta.Aplicacao.Comunicacao;
using HoraCerta.Aplicacao.Comunicacao.Outbox;
using HoraCerta.Aplicacao.Comunicacao.Ports;
using HoraCerta.Infaestrutura.Comunicacao;
using HoraCerta.Infaestrutura.Comunicacao.Outbox;
using HoraCerta.Infaestrutura.Comunicacao.Repositorio;
using HoraCerta.Infaestrutura.Comunicacao.ProvedorTwilio;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HoraCerta.Infaestrutura.Extensions;

public static class ComunicacaoServiceCollectionExtensions
{
    public static IServiceCollection AddHoraCertaComunicacao(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<TwilioOptions>(configuration.GetSection(TwilioOptions.Secao));
        services.Configure<OutboxOptions>(configuration.GetSection(OutboxOptions.Secao));
        services.Configure<WhatsAppOptions>(configuration.GetSection(WhatsAppOptions.Secao));

        services.AddScoped<IMensagemOutboxRepositorio, EfMensagemOutboxRepositorio>();
        services.AddScoped<ISessaoConversaRepositorio, EfSessaoConversaRepositorio>();
        services.AddScoped<IWebhookTwilioProcessadoRepositorio, EfWebhookTwilioProcessadoRepositorio>();
        services.AddScoped<IEnfileiradorMensagemWhatsApp, EnfileiradorMensagemWhatsApp>();
        services.AddSingleton<INormalizadorTelefone, NormalizadorTelefoneE164>();
        services.AddSingleton<IConversaBotLock, ConversaBotLock>();
        services.AddScoped<IHorarioSilenciosoServico, HorarioSilenciosoServico>();
        services.AddHttpClient("TwilioWhatsAppEnviador");

        var twilio = configuration.GetSection(TwilioOptions.Secao).Get<TwilioOptions>() ?? new TwilioOptions();
        if (twilio.Enabled)
            services.AddScoped<IEnviadorWhatsApp, TwilioWhatsAppEnviador>();
        else
            services.AddScoped<IEnviadorWhatsApp, ConsoleEnviadorWhatsApp>();

        return services;
    }
}
