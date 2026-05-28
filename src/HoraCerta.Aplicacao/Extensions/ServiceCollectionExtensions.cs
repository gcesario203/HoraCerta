using HoraCerta.Aplicacao._Shared.Eventos;
using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao.Comunicacao.Eventos;
using HoraCerta.Aplicacao.Integracao.Eventos;
using Microsoft.Extensions.DependencyInjection;

namespace HoraCerta.Aplicacao.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHoraCertaAplicacao(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddScoped<IDomainEventHandler<Dominio.Cliente.Eventos.AgendamentoConfirmadoEvent>,
            AgendamentoConfirmadoAgendarLembreteHandler>();
        services.AddScoped<IDomainEventHandler<Dominio.Cliente.Eventos.AgendamentoCanceladoEvent>,
            AgendamentoCanceladoCancelarLembreteHandler>();
        services.AddScoped<IDomainEventHandler<Dominio.Cliente.Eventos.AgendamentoRemarcadoEvent>,
            AgendamentoRemarcadoReagendarLembreteHandler>();
        services.AddScoped<IDomainEventHandler<Dominio.Cliente.Eventos.AgendamentoConfirmadoEvent>,
            LogAgendamentoConfirmadoHandler>();
        services.AddScoped<IDomainEventHandler<Dominio.Cliente.Eventos.AgendamentoCanceladoEvent>,
            LogAgendamentoCanceladoHandler>();
        services.AddScoped<IDomainEventHandler<Dominio.Cliente.Eventos.AgendamentoRemarcadoEvent>,
            LogAgendamentoRemarcadoHandler>();
        services.AddScoped<IDomainEventHandler<Dominio.Cliente.Eventos.AgendamentoIniciadoEvent>,
            LogAgendamentoIniciadoHandler>();

        services.AddScoped<IDomainEventHandler<Dominio.Cliente.Eventos.AgendamentoConfirmadoEvent>,
            EnviarNotificacaoConfirmacaoWhatsAppHandler>();
        services.AddScoped<IDomainEventHandler<Dominio.Cliente.Eventos.AgendamentoCanceladoEvent>,
            EnviarNotificacaoCancelamentoWhatsAppHandler>();
        services.AddScoped<IDomainEventHandler<Dominio.Cliente.Eventos.AgendamentoRemarcadoEvent>,
            EnviarNotificacaoRemarcacaoWhatsAppHandler>();

        return services;
    }
}
