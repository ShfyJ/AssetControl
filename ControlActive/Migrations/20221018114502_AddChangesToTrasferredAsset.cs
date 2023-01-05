using Microsoft.EntityFrameworkCore.Migrations;

namespace ControlActive.Migrations
{
    public partial class AddChangesToTrasferredAsset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Shares_TransferredAssetId",
                table: "Shares");

            migrationBuilder.DropIndex(
                name: "IX_RealEstates_TransferredAssetId",
                table: "RealEstates");

            migrationBuilder.CreateIndex(
                name: "IX_Shares_TransferredAssetId",
                table: "Shares",
                column: "TransferredAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_TransferredAssetId",
                table: "RealEstates",
                column: "TransferredAssetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Shares_TransferredAssetId",
                table: "Shares");

            migrationBuilder.DropIndex(
                name: "IX_RealEstates_TransferredAssetId",
                table: "RealEstates");

            migrationBuilder.CreateIndex(
                name: "IX_Shares_TransferredAssetId",
                table: "Shares",
                column: "TransferredAssetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_TransferredAssetId",
                table: "RealEstates",
                column: "TransferredAssetId",
                unique: true);
        }
    }
}
