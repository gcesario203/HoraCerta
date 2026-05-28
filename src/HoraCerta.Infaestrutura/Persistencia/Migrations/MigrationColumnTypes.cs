using Microsoft.EntityFrameworkCore.Migrations;

namespace HoraCerta.Infaestrutura.Persistencia.Migrations;

internal static class MigrationColumnTypes
{
    private const string NpgsqlProvider = "Npgsql.EntityFrameworkCore.PostgreSQL";

    public static bool IsPostgres(MigrationBuilder migrationBuilder) =>
        migrationBuilder.ActiveProvider == NpgsqlProvider;

    public static string DateTime(MigrationBuilder migrationBuilder) =>
        IsPostgres(migrationBuilder) ? "timestamp with time zone" : "TEXT";

    public static void AlterTextColumnToTimestamptz(
        MigrationBuilder migrationBuilder,
        string table,
        string column,
        bool nullable = false)
    {
        var emptyValue = nullable
            ? "NULL::timestamp with time zone"
            : "TIMESTAMP WITH TIME ZONE '1970-01-01 00:00:00+00'";

        migrationBuilder.Sql(
            $"""
            DO $migration$
            BEGIN
              IF EXISTS (
                SELECT 1
                FROM information_schema.columns
                WHERE table_schema = 'public'
                  AND table_name = '{table}'
                  AND column_name = '{column}'
                  AND udt_name IN ('text', 'varchar')
              ) THEN
                ALTER TABLE {table}
                ALTER COLUMN "{column}" TYPE timestamp with time zone
                USING (
                  CASE
                    WHEN btrim("{column}"::text) = '' THEN {emptyValue}
                    ELSE "{column}"::timestamp with time zone
                  END
                );
              END IF;
            END
            $migration$;
            """);
    }

    public static void AlterTimestamptzColumnToText(
        MigrationBuilder migrationBuilder,
        string table,
        string column)
    {
        migrationBuilder.Sql(
            $"""
            ALTER TABLE {table}
            ALTER COLUMN "{column}" TYPE text
            USING "{column}"::text;
            """);
    }
}
