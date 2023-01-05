using Microsoft.EntityFrameworkCore.Migrations;

namespace ControlActive.Migrations
{
    public partial class changeOnRealEstate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RealEstates_OneTimePaymentAssets_OnTimePaymentId",
                table: "RealEstates");

            migrationBuilder.DropIndex(
                name: "IX_RealEstates_OnTimePaymentId",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "OnTimePaymentId",
                table: "RealEstates");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_OneTimePaymentAssetId",
                table: "RealEstates",
                column: "OneTimePaymentAssetId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstates_OneTimePaymentAssets_OneTimePaymentAssetId",
                table: "RealEstates",
                column: "OneTimePaymentAssetId",
                principalTable: "OneTimePaymentAssets",
                principalColumn: "OneTimePaymentAssetId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RealEstates_OneTimePaymentAssets_OneTimePaymentAssetId",
                table: "RealEstates");

            migrationBuilder.DropIndex(
                name: "IX_RealEstates_OneTimePaymentAssetId",
                table: "RealEstates");

            migrationBuilder.AddColumn<int>(
                name: "OnTimePaymentId",
                table: "RealEstates",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_OnTimePaymentId",
                table: "RealEstates",
                column: "OnTimePaymentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstates_OneTimePaymentAssets_OnTimePaymentId",
                table: "RealEstates",
                column: "OnTimePaymentId",
                principalTable: "OneTimePaymentAssets",
                principalColumn: "OneTimePaymentAssetId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
