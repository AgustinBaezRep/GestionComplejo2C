using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionComplejo2C.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqlServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Servicios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Costo = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vestuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Disponible = table.Column<bool>(type: "bit", nullable: false),
                    Duchas = table.Column<int>(type: "int", nullable: false),
                    Capacidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vestuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Canchas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Deporte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoPiso = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JugadoresMax = table.Column<int>(type: "int", nullable: false),
                    PrecioPorHora = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VestuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canchas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Canchas_Vestuarios_VestuarioId",
                        column: x => x.VestuarioId,
                        principalTable: "Vestuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CanchaServicio",
                columns: table => new
                {
                    CanchasId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiciosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanchaServicio", x => new { x.CanchasId, x.ServiciosId });
                    table.ForeignKey(
                        name: "FK_CanchaServicio_Canchas_CanchasId",
                        column: x => x.CanchasId,
                        principalTable: "Canchas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CanchaServicio_Servicios_ServiciosId",
                        column: x => x.ServiciosId,
                        principalTable: "Servicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CanchaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Inicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Horas = table.Column<int>(type: "int", nullable: false),
                    Importe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Cancelada = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservas_Canchas_CanchaId",
                        column: x => x.CanchaId,
                        principalTable: "Canchas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Canchas_VestuarioId",
                table: "Canchas",
                column: "VestuarioId",
                unique: true,
                filter: "[VestuarioId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CanchaServicio_ServiciosId",
                table: "CanchaServicio",
                column: "ServiciosId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_CanchaId",
                table: "Reservas",
                column: "CanchaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CanchaServicio");

            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropTable(
                name: "Servicios");

            migrationBuilder.DropTable(
                name: "Canchas");

            migrationBuilder.DropTable(
                name: "Vestuarios");
        }
    }
}
