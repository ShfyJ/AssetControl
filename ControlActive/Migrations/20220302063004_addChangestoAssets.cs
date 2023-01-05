using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ControlActive.Migrations
{
    public partial class addChangestoAssets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OutOfAccountDate",
                table: "Shares",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Shares",
                type: "boolean",
                nullable: false,
                defaultValue: false);


            migrationBuilder.AddColumn<DateTime>(
                name: "OutOfAccountDate",
                table: "RealEstates",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "RealEstates",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OutOfAccountDate",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "OutOfAccountDate",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "RealEstates");

          
        }
    }
}
