using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ControlActive.Migrations
{
    public partial class AddTemplateTableToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.AddColumn<int>(
                name: "TemplateId",
                table: "FileModels",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Template",
                columns: table => new
                {
                    TemplateId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TemplateName = table.Column<string>(type: "text", nullable: true),
                    IsShare = table.Column<bool>(type: "boolean", nullable: false),
                    IsRealEstate = table.Column<bool>(type: "boolean", nullable: false),
                    HasUser = table.Column<bool>(type: "boolean", nullable: false),
                    HasTransferredAssets = table.Column<bool>(type: "boolean", nullable: false),
                    HasAssetEvaluation = table.Column<bool>(type: "boolean", nullable: false),
                    HasAuction = table.Column<bool>(type: "boolean", nullable: false),
                    HasReductionInAsset = table.Column<bool>(type: "boolean", nullable: false),
                    HasOneTimePaymentAsset = table.Column<bool>(type: "boolean", nullable: false),
                    HasInstallmentAsset = table.Column<bool>(type: "boolean", nullable: false),
                    FileId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Template", x => x.TemplateId);
                    table.ForeignKey(
                        name: "FK_Template_FileModels_FileId",
                        column: x => x.FileId,
                        principalTable: "FileModels",
                        principalColumn: "FileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileModels_TemplateId",
                table: "FileModels",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Template_FileId",
                table: "Template",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileModels_Template_TemplateId",
                table: "FileModels",
                column: "TemplateId",
                principalTable: "Template",
                principalColumn: "TemplateId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileModels_Template_TemplateId",
                table: "FileModels");

            migrationBuilder.DropTable(
                name: "Template");

            migrationBuilder.DropIndex(
                name: "IX_FileModels_TemplateId",
                table: "FileModels");

            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "FileModels");

            migrationBuilder.AddColumn<int>(
                name: "TechnicalCharcId",
                table: "RealEstates",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TechnicalCharcs",
                columns: table => new
                {
                    TechnicalCharcId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    TechnicalCharcName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicalCharcs", x => x.TechnicalCharcId);
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
                name: "IX_RealEstates_TechnicalCharcId",
                table: "RealEstates",
                column: "TechnicalCharcId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstateTechnicalCharcs_TechnicalCharcId",
                table: "RealEstateTechnicalCharcs",
                column: "TechnicalCharcId");

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstates_TechnicalCharcs_TechnicalCharcId",
                table: "RealEstates",
                column: "TechnicalCharcId",
                principalTable: "TechnicalCharcs",
                principalColumn: "TechnicalCharcId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
