using HoraCerta.Aplicacao.Agendamento.Handlers;
using HoraCerta.Aplicacao.Autenticacao;
using HoraCerta.Aplicacao.Autenticacao.Handlers;
using HoraCerta.Aplicacao.Cliente.Handlers;
using HoraCerta.Aplicacao.Comunicacao.Bot;
using HoraCerta.Aplicacao.Comunicacao.Handlers;
using HoraCerta.Aplicacao.Estabelecimento.Handlers;
using HoraCerta.Aplicacao.Extensions;
using HoraCerta.Aplicacao.Integracao.Lembretes;
using HoraCerta.Api.Autenticacao;
using HoraCerta.Api.Comunicacao;
using HoraCerta.Infaestrutura.Comunicacao.Outbox;
using HoraCerta.Infaestrutura.Extensions;
using HoraCerta.Infaestrutura.Lembretes;
using HoraCerta.Infaestrutura.Repositorio;
namespace HoraCerta.Api.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddHoraCerta(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionString,
        bool incluirBackgroundLembretes = true)
    {
        services.AddHoraCertaPersistencia(connectionString, configuration);
        services.AddHoraCertaComunicacao(configuration);
        services.AddHoraCertaAplicacao();

        services.Configure<LembreteOptions>(configuration.GetSection(LembreteOptions.Secao));
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.Secao));

        services.AddScoped<ILembreteRepositorio, EfLembreteRepositorio>();
        services.AddScoped<ICredencialProprietarioRepositorio, EfCredencialProprietarioRepositorio>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<CriarClienteHandler>();
        services.AddScoped<BuscarClientePorTelefoneHandler>();
        services.AddScoped<RegistrarOptOutWhatsAppHandler>();
        services.AddScoped<IOrquestradorBotAgendamento, OrquestradorBotAgendamento>();
        services.AddScoped<ProcessarWebhookTwilioHandler>();
        services.AddScoped<TwilioAssinaturaWebhook>();

        services.AddSingleton<Func<string, string>>(_ => SenhaHasher.Hash);
        services.AddSingleton<Func<string, string, bool>>(_ => SenhaHasher.Verificar);

        services.AddScoped<RegistrarCredencialHandler>();
        services.AddScoped<LoginHandler>();

        services.AddScoped<CriarProcedimentoHandler>();
        services.AddScoped<InativarProcedimentoHandler>();
        services.AddScoped<ListarProcedimentosAtivosHandler>();
        services.AddScoped<ListarEstabelecimentosCatalogoHandler>();
        services.AddScoped<ListarSlotsDisponiveisHandler>();
        services.AddScoped<CriarSlotDisponivelHandler>();
        services.AddScoped<IniciarAgendamentoHandler>();
        services.AddScoped<ConfirmarAgendamentoHandler>();
        services.AddScoped<CancelarAgendamentoHandler>();
        services.AddScoped<RemarcarAgendamentoHandler>();
        services.AddScoped<RegistrarAtendimentoHandler>();
        services.AddScoped<AlterarEstadoAtendimentoHandler>();
        services.AddScoped<AvaliarAgendamentoHandler>();
        services.AddScoped<ListarAgendamentosClienteHandler>();
        services.AddScoped<ListarAgendamentosProprietarioHandler>();
        services.AddScoped<ListarAtendimentosHandler>();
        services.AddScoped<ObterAvaliacaoAgendamentoHandler>();

        if (incluirBackgroundLembretes)
        {
            services.AddHostedService<LembreteBackgroundService>();
            services.AddHostedService<OutboxWhatsAppBackgroundService>();
        }

        return services;
    }
}
