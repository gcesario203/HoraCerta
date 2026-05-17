using System.Data;
using HoraCerta.Infaestrutura.Persistencia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace HoraCerta.Infaestrutura.Extensions;

internal static class DatabaseMigrationExtensions
{
    private const string ProductVersion = "8.0.11";

    public static void ApplyHoraCertaMigrations(
        this HoraCertaDbContext context,
        IConfiguration configuration)
    {
        var provider = configuration["Database:Provider"] ?? ServiceCollectionExtensions.ProviderSqlite;

        if (!string.Equals(provider, ServiceCollectionExtensions.ProviderPostgreSql, StringComparison.OrdinalIgnoreCase))
        {
            context.Database.Migrate();
            return;
        }

        var pending = context.Database.GetPendingMigrations().ToList();
        if (pending.Count == 0)
            return;

        var applied = context.Database.GetAppliedMigrations().ToList();
        if (applied.Count == 0 && PostgresSchemaExists(context))
        {
            BaselinePostgresMigrations(context);
            return;
        }

        try
        {
            context.Database.Migrate();
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.DuplicateTable)
        {
            BaselinePostgresMigrations(context);
        }
    }

    private static bool PostgresSchemaExists(HoraCertaDbContext context)
    {
        var connection = context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT EXISTS (
              SELECT 1
              FROM information_schema.tables
              WHERE table_schema = 'public' AND table_name = 'clientes'
            )
            """;
        return Convert.ToBoolean(command.ExecuteScalar());
    }

    private static void BaselinePostgresMigrations(HoraCertaDbContext context)
    {
        foreach (var migrationId in context.Database.GetMigrations())
        {
            context.Database.ExecuteSqlRaw(
                """
                INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
                VALUES ({0}, {1})
                ON CONFLICT ("MigrationId") DO NOTHING
                """,
                migrationId,
                ProductVersion);
        }
    }
}
