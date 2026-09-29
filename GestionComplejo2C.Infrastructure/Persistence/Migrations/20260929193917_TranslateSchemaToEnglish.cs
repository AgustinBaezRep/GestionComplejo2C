using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionComplejo2C.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Renames the Spanish schema to English in place (tables, columns, keys and indexes),
    /// preserving existing data instead of the drop/create that EF scaffolds by default.
    /// </summary>
    public partial class TranslateSchemaToEnglish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_CanchaServicio_Canchas_CanchasId", table: "CanchaServicio");
            migrationBuilder.DropForeignKey(name: "FK_CanchaServicio_Servicios_ServiciosId", table: "CanchaServicio");
            migrationBuilder.DropForeignKey(name: "FK_Canchas_Vestuarios_VestuarioId", table: "Canchas");
            migrationBuilder.DropForeignKey(name: "FK_Reservas_Canchas_CanchaId", table: "Reservas");
            migrationBuilder.DropForeignKey(name: "FK_Reservas_Clientes_ClienteId", table: "Reservas");

            migrationBuilder.DropPrimaryKey(name: "PK_Administradores", table: "Administradores");
            migrationBuilder.DropPrimaryKey(name: "PK_CanchaServicio", table: "CanchaServicio");
            migrationBuilder.DropPrimaryKey(name: "PK_Canchas", table: "Canchas");
            migrationBuilder.DropPrimaryKey(name: "PK_Clientes", table: "Clientes");
            migrationBuilder.DropPrimaryKey(name: "PK_Reservas", table: "Reservas");
            migrationBuilder.DropPrimaryKey(name: "PK_Servicios", table: "Servicios");
            migrationBuilder.DropPrimaryKey(name: "PK_Vestuarios", table: "Vestuarios");

            migrationBuilder.DropIndex(name: "IX_CanchaServicio_ServiciosId", table: "CanchaServicio");
            migrationBuilder.DropIndex(name: "IX_Canchas_VestuarioId", table: "Canchas");

            migrationBuilder.RenameTable(name: "Administradores", newName: "Administrators");
            migrationBuilder.RenameTable(name: "Clientes", newName: "Customers");
            migrationBuilder.RenameTable(name: "Canchas", newName: "Courts");
            migrationBuilder.RenameTable(name: "Reservas", newName: "Bookings");
            migrationBuilder.RenameTable(name: "Servicios", newName: "Amenities");
            migrationBuilder.RenameTable(name: "Vestuarios", newName: "LockerRooms");
            migrationBuilder.RenameTable(name: "CanchaServicio", newName: "AmenityCourt");

            migrationBuilder.RenameColumn(name: "Deporte", table: "Courts", newName: "Sport");
            migrationBuilder.RenameColumn(name: "TipoPiso", table: "Courts", newName: "SurfaceType");
            migrationBuilder.RenameColumn(name: "JugadoresMax", table: "Courts", newName: "MaxPlayers");
            migrationBuilder.RenameColumn(name: "PrecioPorHora", table: "Courts", newName: "PricePerHour");
            migrationBuilder.RenameColumn(name: "VestuarioId", table: "Courts", newName: "LockerRoomId");

            migrationBuilder.RenameColumn(name: "CanchaId", table: "Bookings", newName: "CourtId");
            migrationBuilder.RenameColumn(name: "ClienteId", table: "Bookings", newName: "CustomerId");
            migrationBuilder.RenameColumn(name: "Inicio", table: "Bookings", newName: "Start");
            migrationBuilder.RenameColumn(name: "Horas", table: "Bookings", newName: "Hours");
            migrationBuilder.RenameColumn(name: "Importe", table: "Bookings", newName: "Amount");
            migrationBuilder.RenameColumn(name: "Cancelada", table: "Bookings", newName: "Cancelled");

            migrationBuilder.RenameColumn(name: "Nombre", table: "Amenities", newName: "Name");
            migrationBuilder.RenameColumn(name: "Descripcion", table: "Amenities", newName: "Description");
            migrationBuilder.RenameColumn(name: "Costo", table: "Amenities", newName: "Cost");

            migrationBuilder.RenameColumn(name: "Disponible", table: "LockerRooms", newName: "Available");
            migrationBuilder.RenameColumn(name: "Duchas", table: "LockerRooms", newName: "Showers");
            migrationBuilder.RenameColumn(name: "Capacidad", table: "LockerRooms", newName: "Capacity");

            migrationBuilder.RenameColumn(name: "CanchasId", table: "AmenityCourt", newName: "CourtsId");
            migrationBuilder.RenameColumn(name: "ServiciosId", table: "AmenityCourt", newName: "AmenitiesId");

            migrationBuilder.RenameIndex(name: "IX_Reservas_CanchaId", table: "Bookings", newName: "IX_Bookings_CourtId");
            migrationBuilder.RenameIndex(name: "IX_Reservas_ClienteId", table: "Bookings", newName: "IX_Bookings_CustomerId");

            migrationBuilder.AddPrimaryKey(name: "PK_Administrators", table: "Administrators", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Customers", table: "Customers", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Courts", table: "Courts", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Bookings", table: "Bookings", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Amenities", table: "Amenities", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_LockerRooms", table: "LockerRooms", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_AmenityCourt", table: "AmenityCourt", columns: new[] { "AmenitiesId", "CourtsId" });

            migrationBuilder.CreateIndex(
                name: "IX_AmenityCourt_CourtsId",
                table: "AmenityCourt",
                column: "CourtsId");

            migrationBuilder.CreateIndex(
                name: "IX_Courts_LockerRoomId",
                table: "Courts",
                column: "LockerRoomId",
                unique: true,
                filter: "[LockerRoomId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AmenityCourt_Amenities_AmenitiesId",
                table: "AmenityCourt",
                column: "AmenitiesId",
                principalTable: "Amenities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AmenityCourt_Courts_CourtsId",
                table: "AmenityCourt",
                column: "CourtsId",
                principalTable: "Courts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courts_LockerRooms_LockerRoomId",
                table: "Courts",
                column: "LockerRoomId",
                principalTable: "LockerRooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Courts_CourtId",
                table: "Bookings",
                column: "CourtId",
                principalTable: "Courts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Customers_CustomerId",
                table: "Bookings",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_AmenityCourt_Amenities_AmenitiesId", table: "AmenityCourt");
            migrationBuilder.DropForeignKey(name: "FK_AmenityCourt_Courts_CourtsId", table: "AmenityCourt");
            migrationBuilder.DropForeignKey(name: "FK_Courts_LockerRooms_LockerRoomId", table: "Courts");
            migrationBuilder.DropForeignKey(name: "FK_Bookings_Courts_CourtId", table: "Bookings");
            migrationBuilder.DropForeignKey(name: "FK_Bookings_Customers_CustomerId", table: "Bookings");

            migrationBuilder.DropPrimaryKey(name: "PK_Administrators", table: "Administrators");
            migrationBuilder.DropPrimaryKey(name: "PK_Customers", table: "Customers");
            migrationBuilder.DropPrimaryKey(name: "PK_Courts", table: "Courts");
            migrationBuilder.DropPrimaryKey(name: "PK_Bookings", table: "Bookings");
            migrationBuilder.DropPrimaryKey(name: "PK_Amenities", table: "Amenities");
            migrationBuilder.DropPrimaryKey(name: "PK_LockerRooms", table: "LockerRooms");
            migrationBuilder.DropPrimaryKey(name: "PK_AmenityCourt", table: "AmenityCourt");

            migrationBuilder.DropIndex(name: "IX_AmenityCourt_CourtsId", table: "AmenityCourt");
            migrationBuilder.DropIndex(name: "IX_Courts_LockerRoomId", table: "Courts");

            migrationBuilder.RenameIndex(name: "IX_Bookings_CourtId", table: "Bookings", newName: "IX_Reservas_CanchaId");
            migrationBuilder.RenameIndex(name: "IX_Bookings_CustomerId", table: "Bookings", newName: "IX_Reservas_ClienteId");

            migrationBuilder.RenameColumn(name: "Sport", table: "Courts", newName: "Deporte");
            migrationBuilder.RenameColumn(name: "SurfaceType", table: "Courts", newName: "TipoPiso");
            migrationBuilder.RenameColumn(name: "MaxPlayers", table: "Courts", newName: "JugadoresMax");
            migrationBuilder.RenameColumn(name: "PricePerHour", table: "Courts", newName: "PrecioPorHora");
            migrationBuilder.RenameColumn(name: "LockerRoomId", table: "Courts", newName: "VestuarioId");

            migrationBuilder.RenameColumn(name: "CourtId", table: "Bookings", newName: "CanchaId");
            migrationBuilder.RenameColumn(name: "CustomerId", table: "Bookings", newName: "ClienteId");
            migrationBuilder.RenameColumn(name: "Start", table: "Bookings", newName: "Inicio");
            migrationBuilder.RenameColumn(name: "Hours", table: "Bookings", newName: "Horas");
            migrationBuilder.RenameColumn(name: "Amount", table: "Bookings", newName: "Importe");
            migrationBuilder.RenameColumn(name: "Cancelled", table: "Bookings", newName: "Cancelada");

            migrationBuilder.RenameColumn(name: "Name", table: "Amenities", newName: "Nombre");
            migrationBuilder.RenameColumn(name: "Description", table: "Amenities", newName: "Descripcion");
            migrationBuilder.RenameColumn(name: "Cost", table: "Amenities", newName: "Costo");

            migrationBuilder.RenameColumn(name: "Available", table: "LockerRooms", newName: "Disponible");
            migrationBuilder.RenameColumn(name: "Showers", table: "LockerRooms", newName: "Duchas");
            migrationBuilder.RenameColumn(name: "Capacity", table: "LockerRooms", newName: "Capacidad");

            migrationBuilder.RenameColumn(name: "CourtsId", table: "AmenityCourt", newName: "CanchasId");
            migrationBuilder.RenameColumn(name: "AmenitiesId", table: "AmenityCourt", newName: "ServiciosId");

            migrationBuilder.RenameTable(name: "Administrators", newName: "Administradores");
            migrationBuilder.RenameTable(name: "Customers", newName: "Clientes");
            migrationBuilder.RenameTable(name: "Courts", newName: "Canchas");
            migrationBuilder.RenameTable(name: "Bookings", newName: "Reservas");
            migrationBuilder.RenameTable(name: "Amenities", newName: "Servicios");
            migrationBuilder.RenameTable(name: "LockerRooms", newName: "Vestuarios");
            migrationBuilder.RenameTable(name: "AmenityCourt", newName: "CanchaServicio");

            migrationBuilder.AddPrimaryKey(name: "PK_Administradores", table: "Administradores", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Clientes", table: "Clientes", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Canchas", table: "Canchas", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Reservas", table: "Reservas", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Servicios", table: "Servicios", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Vestuarios", table: "Vestuarios", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_CanchaServicio", table: "CanchaServicio", columns: new[] { "CanchasId", "ServiciosId" });

            migrationBuilder.CreateIndex(
                name: "IX_CanchaServicio_ServiciosId",
                table: "CanchaServicio",
                column: "ServiciosId");

            migrationBuilder.CreateIndex(
                name: "IX_Canchas_VestuarioId",
                table: "Canchas",
                column: "VestuarioId",
                unique: true,
                filter: "[VestuarioId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CanchaServicio_Canchas_CanchasId",
                table: "CanchaServicio",
                column: "CanchasId",
                principalTable: "Canchas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CanchaServicio_Servicios_ServiciosId",
                table: "CanchaServicio",
                column: "ServiciosId",
                principalTable: "Servicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Canchas_Vestuarios_VestuarioId",
                table: "Canchas",
                column: "VestuarioId",
                principalTable: "Vestuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Canchas_CanchaId",
                table: "Reservas",
                column: "CanchaId",
                principalTable: "Canchas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Clientes_ClienteId",
                table: "Reservas",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
