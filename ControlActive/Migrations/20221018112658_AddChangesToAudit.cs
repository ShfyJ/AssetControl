using Microsoft.EntityFrameworkCore.Migrations;

namespace ControlActive.Migrations
{
    public partial class AddChangesToAudit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RealEstateId",
                table: "TransferredAssets",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShareId",
                table: "TransferredAssets",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssetValues",
                table: "AuditLogs",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransferredAssets_RealEstateId",
                table: "TransferredAssets",
                column: "RealEstateId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferredAssets_ShareId",
                table: "TransferredAssets",
                column: "ShareId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferredAssets_RealEstates_RealEstateId",
                table: "TransferredAssets",
                column: "RealEstateId",
                principalTable: "RealEstates",
                principalColumn: "RealEstateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferredAssets_Shares_ShareId",
                table: "TransferredAssets",
                column: "ShareId",
                principalTable: "Shares",
                principalColumn: "ShareId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransferredAssets_RealEstates_RealEstateId",
                table: "TransferredAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferredAssets_Shares_ShareId",
                table: "TransferredAssets");

            migrationBuilder.DropIndex(
                name: "IX_TransferredAssets_RealEstateId",
                table: "TransferredAssets");

            migrationBuilder.DropIndex(
                name: "IX_TransferredAssets_ShareId",
                table: "TransferredAssets");

            migrationBuilder.DropColumn(
                name: "RealEstateId",
                table: "TransferredAssets");

            migrationBuilder.DropColumn(
                name: "ShareId",
                table: "TransferredAssets");

            migrationBuilder.DropColumn(
                name: "AssetValues",
                table: "AuditLogs");
        }
    }
}
