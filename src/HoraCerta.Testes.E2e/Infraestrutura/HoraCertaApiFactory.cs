using HoraCerta.Infaestrutura.Persistencia;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HoraCerta.Testes.E2e.Infraestrutura;

public class HoraCertaApiFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<HoraCertaDbContext>>();
            services.RemoveAll<HoraCertaDbContext>();

            _connection = new SqliteConnection($"Data Source=horacerta-e2e-{Guid.NewGuid():N};Mode=Memory;Cache=Shared");
            _connection.Open();

            services.AddDbContext<HoraCertaDbContext>(options =>
                options.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _connection?.Dispose();

        base.Dispose(disposing);
    }
}
