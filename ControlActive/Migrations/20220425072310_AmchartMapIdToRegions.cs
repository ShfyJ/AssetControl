using Microsoft.EntityFrameworkCore.Migrations;

namespace ControlActive.Migrations
{
    public partial class AmchartMapIdToRegions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AmchartMapId",
                table: "Regions",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmchartMapId",
                table: "Regions");
        }
    }
}
