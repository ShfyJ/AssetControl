using Microsoft.EntityFrameworkCore.Migrations;

namespace ControlActive.Migrations
{
    public partial class ChangeToModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RealEstateTechnicalCharc");

            migrationBuilder.DropColumn(
                name: "OtherExpensesForYear",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "ProductionArea",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "ProfitOrLossOfYear1",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "ProfitOrLossOfYear2",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "ProfitOrLossOfYear3",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "ShareOfActivity",
                table: "RealEstates");

            migrationBuilder.RenameColumn(
                name: "WageForYear",
                table: "RealEstates",
                newName: "MaintenanceCostForYear");

            migrationBuilder.RenameColumn(
                name: "TaxForYear",
                table: "RealEstates",
                newName: "FullArea");

            migrationBuilder.AddColumn<string>(
                name: "MaintanenceCostForYear",
                table: "Shares",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Year1",
                table: "Shares",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Year2",
                table: "Shares",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Year3",
                table: "Shares",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TechnicalCharcId",
                table: "RealEstates",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_TechnicalCharcId",
                table: "RealEstates",
                column: "TechnicalCharcId");

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstates_TechnicalCharcs_TechnicalCharcId",
                table: "RealEstates",
                column: "TechnicalCharcId",
                principalTable: "TechnicalCharcs",
                principalColumn: "TechnicalCharcId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RealEstates_TechnicalCharcs_TechnicalCharcId",
                table: "RealEstates");

            migrationBuilder.DropIndex(
                name: "IX_RealEstates_TechnicalCharcId",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "MaintanenceCostForYear",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "Year1",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "Year2",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "Year3",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "TechnicalCharcId",
                table: "RealEstates");

            migrationBuilder.RenameColumn(
                name: "MaintenanceCostForYear",
                table: "RealEstates",
                newName: "WageForYear");

            migrationBuilder.RenameColumn(
                name: "FullArea",
                table: "RealEstates",
                newName: "TaxForYear");

            migrationBuilder.AddColumn<string>(
                name: "OtherExpensesForYear",
                table: "RealEstates",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductionArea",
                table: "RealEstates",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProfitOrLossOfYear1",
                table: "RealEstates",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProfitOrLossOfYear2",
                table: "RealEstates",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProfitOrLossOfYear3",
                table: "RealEstates",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShareOfActivity",
                table: "RealEstates",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "RealEstateTechnicalCharc",
                columns: table => new
                {
                    RealEstateId = table.Column<int>(type: "integer", nullable: false),
                    TechnicalCharsTechnicalCharcId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealEstateTechnicalCharc", x => new { x.RealEstateId, x.TechnicalCharsTechnicalCharcId });
                    table.ForeignKey(
                        name: "FK_RealEstateTechnicalCharc_RealEstates_RealEstateId",
                        column: x => x.RealEstateId,
                        principalTable: "RealEstates",
                        principalColumn: "RealEstateId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RealEstateTechnicalCharc_TechnicalCharcs_TechnicalCharsTech~",
                        column: x => x.TechnicalCharsTechnicalCharcId,
                        principalTable: "TechnicalCharcs",
                        principalColumn: "TechnicalCharcId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RealEstateTechnicalCharc_TechnicalCharsTechnicalCharcId",
                table: "RealEstateTechnicalCharc",
                column: "TechnicalCharsTechnicalCharcId");
        }
    }
}
