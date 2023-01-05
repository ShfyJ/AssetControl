using Microsoft.EntityFrameworkCore.Migrations;

namespace ControlActive.Migrations
{
    public partial class changestofilemodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FileModels_OneTimePaymentAssetId",
                table: "FileModels");

            migrationBuilder.DropIndex(
                name: "IX_FileModels_OneTimePaymentStep3Id",
                table: "FileModels");

            migrationBuilder.AddColumn<int>(
                name: "InstallmentStep2Id",
                table: "FileModels",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OneTimePaymentStep2Id",
                table: "FileModels",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileModels_InstallmentStep2Id",
                table: "FileModels",
                column: "InstallmentStep2Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileModels_OneTimePaymentAssetId",
                table: "FileModels",
                column: "OneTimePaymentAssetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileModels_OneTimePaymentStep2Id",
                table: "FileModels",
                column: "OneTimePaymentStep2Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileModels_OneTimePaymentStep3Id",
                table: "FileModels",
                column: "OneTimePaymentStep3Id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FileModels_InstallmentStep2_InstallmentStep2Id",
                table: "FileModels",
                column: "InstallmentStep2Id",
                principalTable: "InstallmentStep2",
                principalColumn: "InstallmentStep2Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FileModels_OneTimePaymentStep2_OneTimePaymentStep2Id",
                table: "FileModels",
                column: "OneTimePaymentStep2Id",
                principalTable: "OneTimePaymentStep2",
                principalColumn: "OneTimePaymentStep2Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileModels_InstallmentStep2_InstallmentStep2Id",
                table: "FileModels");

            migrationBuilder.DropForeignKey(
                name: "FK_FileModels_OneTimePaymentStep2_OneTimePaymentStep2Id",
                table: "FileModels");

            migrationBuilder.DropIndex(
                name: "IX_FileModels_InstallmentStep2Id",
                table: "FileModels");

            migrationBuilder.DropIndex(
                name: "IX_FileModels_OneTimePaymentAssetId",
                table: "FileModels");

            migrationBuilder.DropIndex(
                name: "IX_FileModels_OneTimePaymentStep2Id",
                table: "FileModels");

            migrationBuilder.DropIndex(
                name: "IX_FileModels_OneTimePaymentStep3Id",
                table: "FileModels");

            migrationBuilder.DropColumn(
                name: "InstallmentStep2Id",
                table: "FileModels");

            migrationBuilder.DropColumn(
                name: "OneTimePaymentStep2Id",
                table: "FileModels");

            migrationBuilder.CreateIndex(
                name: "IX_FileModels_OneTimePaymentAssetId",
                table: "FileModels",
                column: "OneTimePaymentAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_FileModels_OneTimePaymentStep3Id",
                table: "FileModels",
                column: "OneTimePaymentStep3Id");
        }
    }
}
