using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoraCerta.Infaestrutura.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class FixPostgresDateTimeColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (!MigrationColumnTypes.IsPostgres(migrationBuilder))
                return;

            MigrationColumnTypes.AlterTextColumnToTimestamptz(migrationBuilder, "mensagens_outbox", "ProximaTentativaEm");
            MigrationColumnTypes.AlterTextColumnToTimestamptz(migrationBuilder, "mensagens_outbox", "CriadoEm");
            MigrationColumnTypes.AlterTextColumnToTimestamptz(migrationBuilder, "mensagens_outbox", "EnviadoEm", nullable: true);
            MigrationColumnTypes.AlterTextColumnToTimestamptz(migrationBuilder, "sessoes_conversa", "AtualizadoEm");
            MigrationColumnTypes.AlterTextColumnToTimestamptz(migrationBuilder, "sessoes_conversa", "ExpiraEm");
            MigrationColumnTypes.AlterTextColumnToTimestamptz(migrationBuilder, "webhooks_twilio_processados", "ProcessadoEm");
            MigrationColumnTypes.AlterTextColumnToTimestamptz(migrationBuilder, "lembretes", "SlotInicio");
            MigrationColumnTypes.AlterTextColumnToTimestamptz(migrationBuilder, "lembretes", "EnviarEm");
            MigrationColumnTypes.AlterTextColumnToTimestamptz(migrationBuilder, "lembretes", "EnviadoEm", nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (!MigrationColumnTypes.IsPostgres(migrationBuilder))
                return;

            MigrationColumnTypes.AlterTimestamptzColumnToText(migrationBuilder, "mensagens_outbox", "ProximaTentativaEm");
            MigrationColumnTypes.AlterTimestamptzColumnToText(migrationBuilder, "mensagens_outbox", "CriadoEm");
            MigrationColumnTypes.AlterTimestamptzColumnToText(migrationBuilder, "mensagens_outbox", "EnviadoEm");
            MigrationColumnTypes.AlterTimestamptzColumnToText(migrationBuilder, "sessoes_conversa", "AtualizadoEm");
            MigrationColumnTypes.AlterTimestamptzColumnToText(migrationBuilder, "sessoes_conversa", "ExpiraEm");
            MigrationColumnTypes.AlterTimestamptzColumnToText(migrationBuilder, "webhooks_twilio_processados", "ProcessadoEm");
            MigrationColumnTypes.AlterTimestamptzColumnToText(migrationBuilder, "lembretes", "SlotInicio");
            MigrationColumnTypes.AlterTimestamptzColumnToText(migrationBuilder, "lembretes", "EnviarEm");
            MigrationColumnTypes.AlterTimestamptzColumnToText(migrationBuilder, "lembretes", "EnviadoEm");
        }
    }
}
