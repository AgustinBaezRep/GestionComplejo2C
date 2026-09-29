using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionComplejo2C.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuariosYReservaCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cliente",
                table: "Reservas");

            migrationBuilder.AddColumn<Guid>(
                name: "ClienteId",
                table: "Reservas",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Administradores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administradores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Administradores",
                columns: new[] { "Id", "Email", "PasswordHash" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "admin@gestioncomplejo.com", "$2a$11$AFiIAseqT3wzXpPVvgJEhOQnRM12xYCTZUoD8yxo39XyZLsPlC8M2" });

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "Id", "Email", "PasswordHash" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), "cliente@gestioncomplejo.com", "$2a$11$y97y9Cja5snyPIOLVCGxR.xINwxoKpzV9TgCjCiLeBF1NSxVUhnuW" });

            // Las reservas que ya existian guardaban el cliente como texto libre: se reasignan
            // al cliente sembrado para poder crear la FK sin perder filas.
            migrationBuilder.Sql(
                "UPDATE Reservas SET ClienteId = '22222222-2222-2222-2222-222222222222';");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_ClienteId",
                table: "Reservas",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Clientes_ClienteId",
                table: "Reservas",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Clientes_ClienteId",
                table: "Reservas");

            migrationBuilder.DropTable(
                name: "Administradores");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_ClienteId",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Reservas");

            migrationBuilder.AddColumn<string>(
                name: "Cliente",
                table: "Reservas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
