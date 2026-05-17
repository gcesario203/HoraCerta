using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoraCerta.Infaestrutura.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class Onda2LembretesAuthAvaliacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "credenciais_proprietario",
                columns: table => new
                {
                    ProprietarioId = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_credenciais_proprietario", x => x.ProprietarioId);
                });

            migrationBuilder.CreateTable(
                name: "lembretes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    ProprietarioId = table.Column<string>(type: "TEXT", nullable: false),
                    ClienteId = table.Column<string>(type: "TEXT", nullable: false),
                    AgendamentoId = table.Column<string>(type: "TEXT", nullable: false),
                    TelefoneCliente = table.Column<string>(type: "TEXT", nullable: false),
                    SlotInicio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EnviarEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    EnviadoEm = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lembretes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_credenciais_proprietario_Email",
                table: "credenciais_proprietario",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lembretes_AgendamentoId",
                table: "lembretes",
                column: "AgendamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_lembretes_Status_EnviarEm",
                table: "lembretes",
                columns: new[] { "Status", "EnviarEm" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "credenciais_proprietario");

            migrationBuilder.DropTable(
                name: "lembretes");
        }
    }
}
