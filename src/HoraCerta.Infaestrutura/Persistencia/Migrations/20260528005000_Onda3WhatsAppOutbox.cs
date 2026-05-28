using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoraCerta.Infaestrutura.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class Onda3WhatsAppOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var dateTime = MigrationColumnTypes.DateTime(migrationBuilder);

            migrationBuilder.CreateTable(
                name: "mensagens_outbox",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", nullable: false),
                    TelefoneDestino = table.Column<string>(type: "TEXT", nullable: false),
                    ProprietarioId = table.Column<string>(type: "TEXT", nullable: false),
                    Corpo = table.Column<string>(type: "TEXT", nullable: false),
                    PayloadJson = table.Column<string>(type: "TEXT", nullable: true),
                    IdempotencyKey = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Tentativas = table.Column<int>(type: "INTEGER", nullable: false),
                    ProximaTentativaEm = table.Column<DateTime>(type: dateTime, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: dateTime, nullable: false),
                    EnviadoEm = table.Column<DateTime>(type: dateTime, nullable: true),
                    UltimoErro = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mensagens_outbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sessoes_conversa",
                columns: table => new
                {
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ProprietarioId = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    Passo = table.Column<string>(type: "TEXT", nullable: false),
                    ClienteId = table.Column<string>(type: "TEXT", nullable: true),
                    ProcedimentoId = table.Column<string>(type: "TEXT", nullable: true),
                    SlotHorarioId = table.Column<string>(type: "TEXT", nullable: true),
                    NomePendente = table.Column<string>(type: "TEXT", nullable: true),
                    AtualizadoEm = table.Column<DateTime>(type: dateTime, nullable: false),
                    ExpiraEm = table.Column<DateTime>(type: dateTime, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sessoes_conversa", x => new { x.Telefone, x.ProprietarioId });
                });

            migrationBuilder.CreateTable(
                name: "webhooks_twilio_processados",
                columns: table => new
                {
                    MessageSid = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    ProcessadoEm = table.Column<DateTime>(type: dateTime, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_webhooks_twilio_processados", x => x.MessageSid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mensagens_outbox_IdempotencyKey",
                table: "mensagens_outbox",
                column: "IdempotencyKey");

            migrationBuilder.CreateIndex(
                name: "IX_mensagens_outbox_Status_ProximaTentativaEm",
                table: "mensagens_outbox",
                columns: new[] { "Status", "ProximaTentativaEm" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "webhooks_twilio_processados");

            migrationBuilder.DropTable(
                name: "sessoes_conversa");

            migrationBuilder.DropTable(
                name: "mensagens_outbox");
        }
    }
}
