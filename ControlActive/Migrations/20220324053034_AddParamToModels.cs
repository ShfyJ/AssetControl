using Microsoft.EntityFrameworkCore.Migrations;

namespace ControlActive.Migrations
{
    public partial class AddParamToModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "TransferredAssets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "SubmissionOnBiddings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "ReductionInAssets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "OneTimePaymentStep3",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "OneTimePaymentStep2",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "OneTimePaymentAssets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "InstallmentStep2",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "InstallmentAssets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "AssetEvaluations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "TransferredAssets");

            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "SubmissionOnBiddings");

            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "ReductionInAssets");

            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "OneTimePaymentStep3");

            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "OneTimePaymentStep2");

            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "InstallmentStep2");

            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "InstallmentAssets");

            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "AssetEvaluations");
        }
    }
}
