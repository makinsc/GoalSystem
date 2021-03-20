using Microsoft.EntityFrameworkCore.Migrations;

namespace GoalSystem.Inventario.Backend.Infrastructure.Persistence.Database.Migrations
{
    public partial class IsNotificacionExpiradaEnviada : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNotificacionExpiradaEnviada",
                table: "InventarioItems",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNotificacionExpiradaEnviada",
                table: "InventarioItems");
        }
    }
}
