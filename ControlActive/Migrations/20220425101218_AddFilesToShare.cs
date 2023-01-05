using Microsoft.EntityFrameworkCore.Migrations;

namespace ControlActive.Migrations
{
    public partial class AddFilesToShare : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuditConclusion",
                table: "Shares",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AuditConclusionLink",
                table: "Shares",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BalanceSheetId",
                table: "Shares",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BalanceSheetLink",
                table: "Shares",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FinancialResultId",
                table: "Shares",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FinancialResultLink",
                table: "Shares",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuditConclusion",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "AuditConclusionLink",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "BalanceSheetId",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "BalanceSheetLink",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "FinancialResultId",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "FinancialResultLink",
                table: "Shares");
        }
    }
}
