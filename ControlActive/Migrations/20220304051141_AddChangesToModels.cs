using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ControlActive.Migrations
{
    public partial class AddChangesToModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChangeOnAmountPayedTime",
                table: "OneTimePaymentStep2",
                newName: "ContractCancelledDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "AuctionCancelledDate",
                table: "SubmissionOnBiddings",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StatusChangedDate",
                table: "ReductionInAssets",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "InvoiceFileId",
                table: "OneTimePaymentStep3",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceFileLink",
                table: "OneTimePaymentStep3",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ChangeOnAmountPayedDate",
                table: "OneTimePaymentStep2",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "BiddingCancelledDate",
                table: "OneTimePaymentAssets",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ContractCancelledDate",
                table: "InstallmentAssets",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StatusChangedDate",
                table: "AssetEvaluations",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuctionCancelledDate",
                table: "SubmissionOnBiddings");

            migrationBuilder.DropColumn(
                name: "StatusChangedDate",
                table: "ReductionInAssets");

            migrationBuilder.DropColumn(
                name: "InvoiceFileId",
                table: "OneTimePaymentStep3");

            migrationBuilder.DropColumn(
                name: "InvoiceFileLink",
                table: "OneTimePaymentStep3");

            migrationBuilder.DropColumn(
                name: "ChangeOnAmountPayedDate",
                table: "OneTimePaymentStep2");

            migrationBuilder.DropColumn(
                name: "BiddingCancelledDate",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropColumn(
                name: "ContractCancelledDate",
                table: "InstallmentAssets");

            migrationBuilder.DropColumn(
                name: "StatusChangedDate",
                table: "AssetEvaluations");

            migrationBuilder.RenameColumn(
                name: "ContractCancelledDate",
                table: "OneTimePaymentStep2",
                newName: "ChangeOnAmountPayedTime");
        }
    }
}
