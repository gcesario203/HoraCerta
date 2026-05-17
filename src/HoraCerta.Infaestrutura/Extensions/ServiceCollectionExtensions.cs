using HoraCerta.Dominio.Cliente.Servicos.Repositorio;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;
using HoraCerta.Infaestrutura.Persistencia;
using HoraCerta.Infaestrutura.Repositorio;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HoraCerta.Infaestrutura.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHoraCertaPersistencia(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<HoraCertaDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IProprietarioRepositorio, EfProprietarioRepositorio>();
        services.AddScoped<IClienteRepositorio, EfClienteRepositorio>();

        return services;
    }

    public static void AplicarMigrationsHoraCerta(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<HoraCertaDbContext>();
        context.Database.Migrate();
    }
}
