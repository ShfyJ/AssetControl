using Microsoft.EntityFrameworkCore.Migrations;

namespace ControlActive.Migrations
{
    public partial class ChangeTechnicalCharcs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RealEstates_TechnicalCharcs_TechnicalCharsId",
                table: "RealEstates");

            migrationBuilder.DropIndex(
                name: "IX_RealEstates_TechnicalCharsId",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "TechnicalCharsId",
                table: "RealEstates");

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

            migrationBuilder.CreateTable(
                name: "RealEstateTechnicalCharcs",
                columns: table => new
                {
                    RealEstateId = table.Column<int>(type: "integer", nullable: false),
                    TechnicalCharcId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealEstateTechnicalCharcs", x => new { x.RealEstateId, x.TechnicalCharcId });
                    table.ForeignKey(
                        name: "FK_RealEstateTechnicalCharcs_RealEstates_RealEstateId",
                        column: x => x.RealEstateId,
                        principalTable: "RealEstates",
                        principalColumn: "RealEstateId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RealEstateTechnicalCharcs_TechnicalCharcs_TechnicalCharcId",
                        column: x => x.TechnicalCharcId,
                        principalTable: "TechnicalCharcs",
                        principalColumn: "TechnicalCharcId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RealEstateTechnicalCharc_TechnicalCharsTechnicalCharcId",
                table: "RealEstateTechnicalCharc",
                column: "TechnicalCharsTechnicalCharcId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstateTechnicalCharcs_TechnicalCharcId",
                table: "RealEstateTechnicalCharcs",
                column: "TechnicalCharcId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RealEstateTechnicalCharc");

            migrationBuilder.DropTable(
                name: "RealEstateTechnicalCharcs");

            migrationBuilder.AddColumn<int>(
                name: "TechnicalCharsId",
                table: "RealEstates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_TechnicalCharsId",
                table: "RealEstates",
                column: "TechnicalCharsId");

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstates_TechnicalCharcs_TechnicalCharsId",
                table: "RealEstates",
                column: "TechnicalCharsId",
                principalTable: "TechnicalCharcs",
                principalColumn: "TechnicalCharcId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
