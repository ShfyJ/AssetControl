using Microsoft.EntityFrameworkCore.Migrations;

namespace ControlActive.Migrations
{
    public partial class changesToNavigations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RealEstates_AssetEvaluations_AssetEvaluationId",
                table: "RealEstates");

            migrationBuilder.DropForeignKey(
                name: "FK_RealEstates_InstallmentAssets_InstallmentAssetId",
                table: "RealEstates");

            migrationBuilder.DropForeignKey(
                name: "FK_RealEstates_OneTimePaymentAssets_OneTimePaymentAssetId",
                table: "RealEstates");

            migrationBuilder.DropForeignKey(
                name: "FK_RealEstates_ReductionInAssets_ReductionInAssetId",
                table: "RealEstates");

            migrationBuilder.DropForeignKey(
                name: "FK_RealEstates_SubmissionOnBiddings_SubmissionOnBiddingId",
                table: "RealEstates");

            migrationBuilder.DropForeignKey(
                name: "FK_Shares_AssetEvaluations_AssetEvaluationId",
                table: "Shares");

            migrationBuilder.DropForeignKey(
                name: "FK_Shares_InstallmentAssets_InstallmentAssetId",
                table: "Shares");

            migrationBuilder.DropForeignKey(
                name: "FK_Shares_OneTimePaymentAssets_OnTimePaymentId",
                table: "Shares");

            migrationBuilder.DropForeignKey(
                name: "FK_Shares_ReductionInAssets_ReductionInAssetId",
                table: "Shares");

            migrationBuilder.DropForeignKey(
                name: "FK_Shares_SubmissionOnBiddings_SubmissionOnBiddingId",
                table: "Shares");

            migrationBuilder.DropIndex(
                name: "IX_Shares_AssetEvaluationId",
                table: "Shares");

            migrationBuilder.DropIndex(
                name: "IX_Shares_InstallmentAssetId",
                table: "Shares");

            migrationBuilder.DropIndex(
                name: "IX_Shares_OnTimePaymentId",
                table: "Shares");

            migrationBuilder.DropIndex(
                name: "IX_Shares_ReductionInAssetId",
                table: "Shares");

            migrationBuilder.DropIndex(
                name: "IX_Shares_SubmissionOnBiddingId",
                table: "Shares");

            migrationBuilder.DropIndex(
                name: "IX_RealEstates_AssetEvaluationId",
                table: "RealEstates");

            migrationBuilder.DropIndex(
                name: "IX_RealEstates_InstallmentAssetId",
                table: "RealEstates");

            migrationBuilder.DropIndex(
                name: "IX_RealEstates_OneTimePaymentAssetId",
                table: "RealEstates");

            migrationBuilder.DropIndex(
                name: "IX_RealEstates_ReductionInAssetId",
                table: "RealEstates");

            migrationBuilder.DropIndex(
                name: "IX_RealEstates_SubmissionOnBiddingId",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "AssetEvaluationId",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "InstallmentAssetId",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "OnTimePaymentId",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "OneTimePaymentAssetId",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "ReductionInAssetId",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "SubmissionOnBiddingId",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "AssetEvaluationId",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "InstallmentAssetId",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "OneTimePaymentAssetId",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "ReductionInAssetId",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "SubmissionOnBiddingId",
                table: "RealEstates");

            migrationBuilder.AddColumn<int>(
                name: "RealEstateId",
                table: "SubmissionOnBiddings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShareId",
                table: "SubmissionOnBiddings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "SubmissionOnBiddings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RealEstateId",
                table: "ReductionInAssets",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShareId",
                table: "ReductionInAssets",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CadastreNumber",
                table: "RealEstates",
                type: "character varying(28)",
                maxLength: 28,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(14)",
                oldMaxLength: 14);

            migrationBuilder.AddColumn<int>(
                name: "RealEstateId",
                table: "OneTimePaymentAssets",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShareId",
                table: "OneTimePaymentAssets",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RealEstateId",
                table: "InstallmentAssets",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShareId",
                table: "InstallmentAssets",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RealEstateId",
                table: "AssetEvaluations",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShareId",
                table: "AssetEvaluations",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "AssetEvaluations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionOnBiddings_RealEstateId",
                table: "SubmissionOnBiddings",
                column: "RealEstateId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionOnBiddings_ShareId",
                table: "SubmissionOnBiddings",
                column: "ShareId");

            migrationBuilder.CreateIndex(
                name: "IX_ReductionInAssets_RealEstateId",
                table: "ReductionInAssets",
                column: "RealEstateId");

            migrationBuilder.CreateIndex(
                name: "IX_ReductionInAssets_ShareId",
                table: "ReductionInAssets",
                column: "ShareId");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePaymentAssets_RealEstateId",
                table: "OneTimePaymentAssets",
                column: "RealEstateId");

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePaymentAssets_ShareId",
                table: "OneTimePaymentAssets",
                column: "ShareId");

            migrationBuilder.CreateIndex(
                name: "IX_InstallmentAssets_RealEstateId",
                table: "InstallmentAssets",
                column: "RealEstateId");

            migrationBuilder.CreateIndex(
                name: "IX_InstallmentAssets_ShareId",
                table: "InstallmentAssets",
                column: "ShareId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetEvaluations_RealEstateId",
                table: "AssetEvaluations",
                column: "RealEstateId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetEvaluations_ShareId",
                table: "AssetEvaluations",
                column: "ShareId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetEvaluations_RealEstates_RealEstateId",
                table: "AssetEvaluations",
                column: "RealEstateId",
                principalTable: "RealEstates",
                principalColumn: "RealEstateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetEvaluations_Shares_ShareId",
                table: "AssetEvaluations",
                column: "ShareId",
                principalTable: "Shares",
                principalColumn: "ShareId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InstallmentAssets_RealEstates_RealEstateId",
                table: "InstallmentAssets",
                column: "RealEstateId",
                principalTable: "RealEstates",
                principalColumn: "RealEstateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InstallmentAssets_Shares_ShareId",
                table: "InstallmentAssets",
                column: "ShareId",
                principalTable: "Shares",
                principalColumn: "ShareId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OneTimePaymentAssets_RealEstates_RealEstateId",
                table: "OneTimePaymentAssets",
                column: "RealEstateId",
                principalTable: "RealEstates",
                principalColumn: "RealEstateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OneTimePaymentAssets_Shares_ShareId",
                table: "OneTimePaymentAssets",
                column: "ShareId",
                principalTable: "Shares",
                principalColumn: "ShareId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReductionInAssets_RealEstates_RealEstateId",
                table: "ReductionInAssets",
                column: "RealEstateId",
                principalTable: "RealEstates",
                principalColumn: "RealEstateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReductionInAssets_Shares_ShareId",
                table: "ReductionInAssets",
                column: "ShareId",
                principalTable: "Shares",
                principalColumn: "ShareId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionOnBiddings_RealEstates_RealEstateId",
                table: "SubmissionOnBiddings",
                column: "RealEstateId",
                principalTable: "RealEstates",
                principalColumn: "RealEstateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionOnBiddings_Shares_ShareId",
                table: "SubmissionOnBiddings",
                column: "ShareId",
                principalTable: "Shares",
                principalColumn: "ShareId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetEvaluations_RealEstates_RealEstateId",
                table: "AssetEvaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetEvaluations_Shares_ShareId",
                table: "AssetEvaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_InstallmentAssets_RealEstates_RealEstateId",
                table: "InstallmentAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_InstallmentAssets_Shares_ShareId",
                table: "InstallmentAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_OneTimePaymentAssets_RealEstates_RealEstateId",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_OneTimePaymentAssets_Shares_ShareId",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_ReductionInAssets_RealEstates_RealEstateId",
                table: "ReductionInAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_ReductionInAssets_Shares_ShareId",
                table: "ReductionInAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionOnBiddings_RealEstates_RealEstateId",
                table: "SubmissionOnBiddings");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionOnBiddings_Shares_ShareId",
                table: "SubmissionOnBiddings");

            migrationBuilder.DropIndex(
                name: "IX_SubmissionOnBiddings_RealEstateId",
                table: "SubmissionOnBiddings");

            migrationBuilder.DropIndex(
                name: "IX_SubmissionOnBiddings_ShareId",
                table: "SubmissionOnBiddings");

            migrationBuilder.DropIndex(
                name: "IX_ReductionInAssets_RealEstateId",
                table: "ReductionInAssets");

            migrationBuilder.DropIndex(
                name: "IX_ReductionInAssets_ShareId",
                table: "ReductionInAssets");

            migrationBuilder.DropIndex(
                name: "IX_OneTimePaymentAssets_RealEstateId",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropIndex(
                name: "IX_OneTimePaymentAssets_ShareId",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropIndex(
                name: "IX_InstallmentAssets_RealEstateId",
                table: "InstallmentAssets");

            migrationBuilder.DropIndex(
                name: "IX_InstallmentAssets_ShareId",
                table: "InstallmentAssets");

            migrationBuilder.DropIndex(
                name: "IX_AssetEvaluations_RealEstateId",
                table: "AssetEvaluations");

            migrationBuilder.DropIndex(
                name: "IX_AssetEvaluations_ShareId",
                table: "AssetEvaluations");

            migrationBuilder.DropColumn(
                name: "RealEstateId",
                table: "SubmissionOnBiddings");

            migrationBuilder.DropColumn(
                name: "ShareId",
                table: "SubmissionOnBiddings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "SubmissionOnBiddings");

            migrationBuilder.DropColumn(
                name: "RealEstateId",
                table: "ReductionInAssets");

            migrationBuilder.DropColumn(
                name: "ShareId",
                table: "ReductionInAssets");

            migrationBuilder.DropColumn(
                name: "RealEstateId",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropColumn(
                name: "ShareId",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropColumn(
                name: "RealEstateId",
                table: "InstallmentAssets");

            migrationBuilder.DropColumn(
                name: "ShareId",
                table: "InstallmentAssets");

            migrationBuilder.DropColumn(
                name: "RealEstateId",
                table: "AssetEvaluations");

            migrationBuilder.DropColumn(
                name: "ShareId",
                table: "AssetEvaluations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AssetEvaluations");

            migrationBuilder.AddColumn<int>(
                name: "AssetEvaluationId",
                table: "Shares",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstallmentAssetId",
                table: "Shares",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OnTimePaymentId",
                table: "Shares",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OneTimePaymentAssetId",
                table: "Shares",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReductionInAssetId",
                table: "Shares",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubmissionOnBiddingId",
                table: "Shares",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CadastreNumber",
                table: "RealEstates",
                type: "character varying(14)",
                maxLength: 14,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(28)",
                oldMaxLength: 28);

            migrationBuilder.AddColumn<int>(
                name: "AssetEvaluationId",
                table: "RealEstates",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstallmentAssetId",
                table: "RealEstates",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OneTimePaymentAssetId",
                table: "RealEstates",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReductionInAssetId",
                table: "RealEstates",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubmissionOnBiddingId",
                table: "RealEstates",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shares_AssetEvaluationId",
                table: "Shares",
                column: "AssetEvaluationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shares_InstallmentAssetId",
                table: "Shares",
                column: "InstallmentAssetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shares_OnTimePaymentId",
                table: "Shares",
                column: "OnTimePaymentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shares_ReductionInAssetId",
                table: "Shares",
                column: "ReductionInAssetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shares_SubmissionOnBiddingId",
                table: "Shares",
                column: "SubmissionOnBiddingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_AssetEvaluationId",
                table: "RealEstates",
                column: "AssetEvaluationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_InstallmentAssetId",
                table: "RealEstates",
                column: "InstallmentAssetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_OneTimePaymentAssetId",
                table: "RealEstates",
                column: "OneTimePaymentAssetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_ReductionInAssetId",
                table: "RealEstates",
                column: "ReductionInAssetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_SubmissionOnBiddingId",
                table: "RealEstates",
                column: "SubmissionOnBiddingId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstates_AssetEvaluations_AssetEvaluationId",
                table: "RealEstates",
                column: "AssetEvaluationId",
                principalTable: "AssetEvaluations",
                principalColumn: "AssetEvaluationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstates_InstallmentAssets_InstallmentAssetId",
                table: "RealEstates",
                column: "InstallmentAssetId",
                principalTable: "InstallmentAssets",
                principalColumn: "InstallmentAssetId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstates_OneTimePaymentAssets_OneTimePaymentAssetId",
                table: "RealEstates",
                column: "OneTimePaymentAssetId",
                principalTable: "OneTimePaymentAssets",
                principalColumn: "OneTimePaymentAssetId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstates_ReductionInAssets_ReductionInAssetId",
                table: "RealEstates",
                column: "ReductionInAssetId",
                principalTable: "ReductionInAssets",
                principalColumn: "ReductionInAssetId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstates_SubmissionOnBiddings_SubmissionOnBiddingId",
                table: "RealEstates",
                column: "SubmissionOnBiddingId",
                principalTable: "SubmissionOnBiddings",
                principalColumn: "SubmissionOnBiddingId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shares_AssetEvaluations_AssetEvaluationId",
                table: "Shares",
                column: "AssetEvaluationId",
                principalTable: "AssetEvaluations",
                principalColumn: "AssetEvaluationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shares_InstallmentAssets_InstallmentAssetId",
                table: "Shares",
                column: "InstallmentAssetId",
                principalTable: "InstallmentAssets",
                principalColumn: "InstallmentAssetId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shares_OneTimePaymentAssets_OnTimePaymentId",
                table: "Shares",
                column: "OnTimePaymentId",
                principalTable: "OneTimePaymentAssets",
                principalColumn: "OneTimePaymentAssetId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shares_ReductionInAssets_ReductionInAssetId",
                table: "Shares",
                column: "ReductionInAssetId",
                principalTable: "ReductionInAssets",
                principalColumn: "ReductionInAssetId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shares_SubmissionOnBiddings_SubmissionOnBiddingId",
                table: "Shares",
                column: "SubmissionOnBiddingId",
                principalTable: "SubmissionOnBiddings",
                principalColumn: "SubmissionOnBiddingId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
