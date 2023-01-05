using Microsoft.EntityFrameworkCore.Migrations;

namespace ControlActive.Migrations
{
    public partial class AddchangestoUSer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isPasswordReset",
                table: "AspNetUsers",
                newName: "isPasswordRenewed");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isPasswordRenewed",
                table: "AspNetUsers",
                newName: "isPasswordReset");
        }
    }
}
