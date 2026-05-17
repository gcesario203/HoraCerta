using HoraCerta.Aplicacao.Agendamento.Handlers;
using HoraCerta.Aplicacao.Autenticacao;
using HoraCerta.Aplicacao.Autenticacao.Handlers;
using HoraCerta.Aplicacao.Estabelecimento.Handlers;
using HoraCerta.Aplicacao.Extensions;
using HoraCerta.Aplicacao.Integracao.Lembretes;
using HoraCerta.Api.Autenticacao;
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
        services.AddHoraCertaAplicacao();

        services.Configure<LembreteOptions>(configuration.GetSection(LembreteOptions.Secao));
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.Secao));

        services.AddScoped<ILembreteRepositorio, EfLembreteRepositorio>();
        services.AddScoped<ICredencialProprietarioRepositorio, EfCredencialProprietarioRepositorio>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IEnviadorLembrete, ConsoleEnviadorLembrete>();

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
            services.AddHostedService<LembreteBackgroundService>();

        return services;
    }
}
