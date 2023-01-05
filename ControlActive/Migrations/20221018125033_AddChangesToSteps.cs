using Microsoft.EntityFrameworkCore.Migrations;

namespace ControlActive.Migrations
{
    public partial class AddChangesToSteps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OneTimePaymentAssets_OneTimePaymentStep2Id",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropIndex(
                name: "IX_OneTimePaymentAssets_OneTimePaymentStep3Id",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropIndex(
                name: "IX_InstallmentAssets_InstallmentStep2Id",
                table: "InstallmentAssets");

            migrationBuilder.AddColumn<int>(
                name: "OneTimePaymentAssetId",
                table: "OneTimePaymentStep3",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OneTimePaymentAssetId",
                table: "OneTimePaymentStep2",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InstallmentAssetId",
                table: "InstallmentStep2",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePaymentStep3_OneTimePaymentAssetId",
                table: "OneTimePaymentStep3",
                column: "OneTimePaymentAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePaymentStep2_OneTimePaymentAssetId",
                table: "OneTimePaymentStep2",
                column: "OneTimePaymentAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePaymentAssets_OneTimePaymentStep2Id",
                table: "OneTimePaymentAssets",
                column: "OneTimePaymentStep2Id");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePaymentAssets_OneTimePaymentStep3Id",
                table: "OneTimePaymentAssets",
                column: "OneTimePaymentStep3Id");

            migrationBuilder.CreateIndex(
                name: "IX_InstallmentStep2_InstallmentAssetId",
                table: "InstallmentStep2",
                column: "InstallmentAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_InstallmentAssets_InstallmentStep2Id",
                table: "InstallmentAssets",
                column: "InstallmentStep2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InstallmentStep2_InstallmentAssets_InstallmentAssetId",
                table: "InstallmentStep2",
                column: "InstallmentAssetId",
                principalTable: "InstallmentAssets",
                principalColumn: "InstallmentAssetId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OneTimePaymentStep2_OneTimePaymentAssets_OneTimePaymentAsse~",
                table: "OneTimePaymentStep2",
                column: "OneTimePaymentAssetId",
                principalTable: "OneTimePaymentAssets",
                principalColumn: "OneTimePaymentAssetId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OneTimePaymentStep3_OneTimePaymentAssets_OneTimePaymentAsse~",
                table: "OneTimePaymentStep3",
                column: "OneTimePaymentAssetId",
                principalTable: "OneTimePaymentAssets",
                principalColumn: "OneTimePaymentAssetId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstallmentStep2_InstallmentAssets_InstallmentAssetId",
                table: "InstallmentStep2");

            migrationBuilder.DropForeignKey(
                name: "FK_OneTimePaymentStep2_OneTimePaymentAssets_OneTimePaymentAsse~",
                table: "OneTimePaymentStep2");

            migrationBuilder.DropForeignKey(
                name: "FK_OneTimePaymentStep3_OneTimePaymentAssets_OneTimePaymentAsse~",
                table: "OneTimePaymentStep3");

            migrationBuilder.DropIndex(
                name: "IX_OneTimePaymentStep3_OneTimePaymentAssetId",
                table: "OneTimePaymentStep3");

            migrationBuilder.DropIndex(
                name: "IX_OneTimePaymentStep2_OneTimePaymentAssetId",
                table: "OneTimePaymentStep2");

            migrationBuilder.DropIndex(
                name: "IX_OneTimePaymentAssets_OneTimePaymentStep2Id",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropIndex(
                name: "IX_OneTimePaymentAssets_OneTimePaymentStep3Id",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropIndex(
                name: "IX_InstallmentStep2_InstallmentAssetId",
                table: "InstallmentStep2");

            migrationBuilder.DropIndex(
                name: "IX_InstallmentAssets_InstallmentStep2Id",
                table: "InstallmentAssets");

            migrationBuilder.DropColumn(
                name: "OneTimePaymentAssetId",
                table: "OneTimePaymentStep3");

            migrationBuilder.DropColumn(
                name: "OneTimePaymentAssetId",
                table: "OneTimePaymentStep2");

            migrationBuilder.DropColumn(
                name: "InstallmentAssetId",
                table: "InstallmentStep2");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePaymentAssets_OneTimePaymentStep2Id",
                table: "OneTimePaymentAssets",
                column: "OneTimePaymentStep2Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePaymentAssets_OneTimePaymentStep3Id",
                table: "OneTimePaymentAssets",
                column: "OneTimePaymentStep3Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstallmentAssets_InstallmentStep2Id",
                table: "InstallmentAssets",
                column: "InstallmentStep2Id",
                unique: true);
        }
    }
}
