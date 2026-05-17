using HoraCerta.Dominio.Cliente.Servicos.Repositorio;
using HoraCerta.Dominio.Proprietario.Servicos.Repositorio;
using HoraCerta.Infaestrutura.Persistencia;
using HoraCerta.Infaestrutura.Repositorio;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HoraCerta.Infaestrutura.Extensions;

public static class ServiceCollectionExtensions
{
    public const string ProviderSqlite = "Sqlite";
    public const string ProviderPostgreSql = "PostgreSQL";

    public static IServiceCollection AddHoraCertaPersistencia(
        this IServiceCollection services,
        string connectionString,
        IConfiguration configuration)
    {
        var provider = configuration["Database:Provider"] ?? ProviderSqlite;

        services.AddDbContext<HoraCertaDbContext>(options =>
        {
            if (string.Equals(provider, ProviderPostgreSql, StringComparison.OrdinalIgnoreCase))
                options.UseNpgsql(connectionString);
            else
                options.UseSqlite(connectionString);
        });

        services.AddScoped<IProprietarioRepositorio, EfProprietarioRepositorio>();
        services.AddScoped<IClienteRepositorio, EfClienteRepositorio>();

        return services;
    }

    public static void AplicarMigrationsHoraCerta(this IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<HoraCertaDbContext>();
        var provider = configuration["Database:Provider"] ?? ProviderSqlite;

        if (string.Equals(provider, ProviderPostgreSql, StringComparison.OrdinalIgnoreCase))
            context.Database.EnsureCreated();
        else
            context.Database.Migrate();
    }
}
