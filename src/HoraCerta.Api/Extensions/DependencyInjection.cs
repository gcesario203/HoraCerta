using HoraCerta.Aplicacao._Shared.Eventos;
using HoraCerta.Aplicacao._Shared.Interfaces;
using HoraCerta.Aplicacao.Agendamento.Handlers;
using HoraCerta.Aplicacao.Estabelecimento.Handlers;
using HoraCerta.Infaestrutura.Extensions;

namespace HoraCerta.Api.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddHoraCerta(this IServiceCollection services, string connectionString)
    {
        services.AddHoraCertaPersistencia(connectionString);
        services.AddSingleton<IDomainEventDispatcher, NopDomainEventDispatcher>();

        services.AddScoped<CriarProcedimentoHandler>();
        services.AddScoped<InativarProcedimentoHandler>();
        services.AddScoped<ListarProcedimentosAtivosHandler>();
        services.AddScoped<ListarSlotsDisponiveisHandler>();
        services.AddScoped<CriarSlotDisponivelHandler>();
        services.AddScoped<IniciarAgendamentoHandler>();
        services.AddScoped<ConfirmarAgendamentoHandler>();
        services.AddScoped<CancelarAgendamentoHandler>();
        services.AddScoped<RemarcarAgendamentoHandler>();
        services.AddScoped<RegistrarAtendimentoHandler>();

        return services;
    }
}
