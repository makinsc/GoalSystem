using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoalSystem.Inventario.Backend.Infrastructure.Persistence.Database.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventarioItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FrechaCreacion = table.Column<DateTime>(nullable: false),
                    FrechaModificacion = table.Column<DateTime>(nullable: false),
                    UsusarioCreacion = table.Column<string>(nullable: true),
                    UsusarioModificacion = table.Column<string>(nullable: true),
                    Nombre = table.Column<string>(nullable: true),
                    FechaCaducidad = table.Column<DateTime>(nullable: false),
                    Unidades = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioItems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "Ind_InventarioItem_Nombre_1793F16CC77D03694AF5E1610A43F634",
                table: "InventarioItems",
                column: "Nombre");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventarioItems");
        }
    }
}
