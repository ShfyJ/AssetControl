using Microsoft.EntityFrameworkCore.Migrations;

namespace ControlActive.Migrations
{
    public partial class changestoOnetimeAsset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OneTimePaymentAssets_OneTimePaymentStep2_OneTimePaymentStep~",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_OneTimePaymentAssets_OneTimePaymentStep3_OneTimePaymentStep~",
                table: "OneTimePaymentAssets");

            migrationBuilder.AlterColumn<int>(
                name: "OneTimePaymentStep3Id",
                table: "OneTimePaymentAssets",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "OneTimePaymentStep2Id",
                table: "OneTimePaymentAssets",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_OneTimePaymentAssets_OneTimePaymentStep2_OneTimePaymentStep~",
                table: "OneTimePaymentAssets",
                column: "OneTimePaymentStep2Id",
                principalTable: "OneTimePaymentStep2",
                principalColumn: "OneTimePaymentStep2Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OneTimePaymentAssets_OneTimePaymentStep3_OneTimePaymentStep~",
                table: "OneTimePaymentAssets",
                column: "OneTimePaymentStep3Id",
                principalTable: "OneTimePaymentStep3",
                principalColumn: "OneTimePaymentStep3Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OneTimePaymentAssets_OneTimePaymentStep2_OneTimePaymentStep~",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_OneTimePaymentAssets_OneTimePaymentStep3_OneTimePaymentStep~",
                table: "OneTimePaymentAssets");

            migrationBuilder.AlterColumn<int>(
                name: "OneTimePaymentStep3Id",
                table: "OneTimePaymentAssets",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OneTimePaymentStep2Id",
                table: "OneTimePaymentAssets",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OneTimePaymentAssets_OneTimePaymentStep2_OneTimePaymentStep~",
                table: "OneTimePaymentAssets",
                column: "OneTimePaymentStep2Id",
                principalTable: "OneTimePaymentStep2",
                principalColumn: "OneTimePaymentStep2Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OneTimePaymentAssets_OneTimePaymentStep3_OneTimePaymentStep~",
                table: "OneTimePaymentAssets",
                column: "OneTimePaymentStep3Id",
                principalTable: "OneTimePaymentStep3",
                principalColumn: "OneTimePaymentStep3Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
