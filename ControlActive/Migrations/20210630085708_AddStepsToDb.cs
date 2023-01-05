using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ControlActive.Migrations
{
    public partial class AddStepsToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReductionInAssets_FileModels_FileId",
                table: "ReductionInAssets");

            migrationBuilder.DropIndex(
                name: "IX_ReductionInAssets_FileId",
                table: "ReductionInAssets");

            migrationBuilder.DropIndex(
                name: "IX_FileModels_ReductionInAssetId",
                table: "FileModels");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "ReductionInAssets");

            migrationBuilder.DropColumn(
                name: "IsInstallment",
                table: "ReductionInAssets");

            migrationBuilder.DropColumn(
                name: "ActAndAssetDate",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropColumn(
                name: "ActAndAssetFileLink",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropColumn(
                name: "ActAndAssetNumber",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropColumn(
                name: "AggreementDate",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropColumn(
                name: "AggreementFileLink",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropColumn(
                name: "AggreementNumber",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropColumn(
                name: "AmountOfAssetSold",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropColumn(
                name: "AmountPayed",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropColumn(
                name: "AssetBuyerName",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropColumn(
                name: "Percentage",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropColumn(
                name: "ActAndAssetDate",
                table: "InstallmentAssets");

            migrationBuilder.DropColumn(
                name: "ActAndAssetFileId",
                table: "InstallmentAssets");

            migrationBuilder.DropColumn(
                name: "ActAndAssetFileLink",
                table: "InstallmentAssets");

            migrationBuilder.DropColumn(
                name: "ActAndAssetNumber",
                table: "InstallmentAssets");

            migrationBuilder.RenameColumn(
                name: "AggreementFileId",
                table: "OneTimePaymentAssets",
                newName: "OneTimePaymentStep3Id");

            migrationBuilder.RenameColumn(
                name: "ActAndAssetFileId",
                table: "OneTimePaymentAssets",
                newName: "OneTimePaymentStep2Id");

            migrationBuilder.AlterColumn<int>(
                name: "SolutionFileId",
                table: "InstallmentAssets",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "AggreementFileId",
                table: "InstallmentAssets",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "InstallmentStep2Id",
                table: "InstallmentAssets",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OneTimePaymentStep3Id",
                table: "FileModels",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InstallmentStep2",
                columns: table => new
                {
                    InstallmentStep2Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActAndAssetDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ActAndAssetNumber = table.Column<string>(type: "text", nullable: true),
                    ActAndAssetFileLink = table.Column<string>(type: "text", nullable: true),
                    ActAndAssetFileId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstallmentStep2", x => x.InstallmentStep2Id);
                });

            migrationBuilder.CreateTable(
                name: "OneTimePaymentStep2",
                columns: table => new
                {
                    OneTimePaymentStep2Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AssetBuyerName = table.Column<string>(type: "text", nullable: true),
                    AmountOfAssetSold = table.Column<float>(type: "real", nullable: false),
                    AggreementDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AggreementNumber = table.Column<string>(type: "text", nullable: true),
                    AggreementFileLink = table.Column<string>(type: "text", nullable: true),
                    AggreementFileId = table.Column<int>(type: "integer", nullable: false),
                    AmountPayed = table.Column<float>(type: "real", nullable: true),
                    ChangeOnAmountPayedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneTimePaymentStep2", x => x.OneTimePaymentStep2Id);
                });

            migrationBuilder.CreateTable(
                name: "OneTimePaymentStep3",
                columns: table => new
                {
                    OneTimePaymentStep3Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActAndAssetDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ActAndAssetNumber = table.Column<string>(type: "text", nullable: true),
                    ActAndAssetFileLink = table.Column<string>(type: "text", nullable: true),
                    ActAndAssetFileId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneTimePaymentStep3", x => x.OneTimePaymentStep3Id);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_FileModels_OneTimePaymentStep3Id",
                table: "FileModels",
                column: "OneTimePaymentStep3Id");

            migrationBuilder.CreateIndex(
                name: "IX_FileModels_ReductionInAssetId",
                table: "FileModels",
                column: "ReductionInAssetId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FileModels_OneTimePaymentStep3_OneTimePaymentStep3Id",
                table: "FileModels",
                column: "OneTimePaymentStep3Id",
                principalTable: "OneTimePaymentStep3",
                principalColumn: "OneTimePaymentStep3Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InstallmentAssets_InstallmentStep2_InstallmentStep2Id",
                table: "InstallmentAssets",
                column: "InstallmentStep2Id",
                principalTable: "InstallmentStep2",
                principalColumn: "InstallmentStep2Id",
                onDelete: ReferentialAction.Restrict);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileModels_OneTimePaymentStep3_OneTimePaymentStep3Id",
                table: "FileModels");

            migrationBuilder.DropForeignKey(
                name: "FK_InstallmentAssets_InstallmentStep2_InstallmentStep2Id",
                table: "InstallmentAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_OneTimePaymentAssets_OneTimePaymentStep2_OneTimePaymentStep~",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_OneTimePaymentAssets_OneTimePaymentStep3_OneTimePaymentStep~",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropTable(
                name: "InstallmentStep2");

            migrationBuilder.DropTable(
                name: "OneTimePaymentStep2");

            migrationBuilder.DropTable(
                name: "OneTimePaymentStep3");

            migrationBuilder.DropIndex(
                name: "IX_OneTimePaymentAssets_OneTimePaymentStep2Id",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropIndex(
                name: "IX_OneTimePaymentAssets_OneTimePaymentStep3Id",
                table: "OneTimePaymentAssets");

            migrationBuilder.DropIndex(
                name: "IX_InstallmentAssets_InstallmentStep2Id",
                table: "InstallmentAssets");

            migrationBuilder.DropIndex(
                name: "IX_FileModels_OneTimePaymentStep3Id",
                table: "FileModels");

            migrationBuilder.DropIndex(
                name: "IX_FileModels_ReductionInAssetId",
                table: "FileModels");

            migrationBuilder.DropColumn(
                name: "InstallmentStep2Id",
                table: "InstallmentAssets");

            migrationBuilder.DropColumn(
                name: "OneTimePaymentStep3Id",
                table: "FileModels");

            migrationBuilder.RenameColumn(
                name: "OneTimePaymentStep3Id",
                table: "OneTimePaymentAssets",
                newName: "AggreementFileId");

            migrationBuilder.RenameColumn(
                name: "OneTimePaymentStep2Id",
                table: "OneTimePaymentAssets",
                newName: "ActAndAssetFileId");

            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "ReductionInAssets",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInstallment",
                table: "ReductionInAssets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActAndAssetDate",
                table: "OneTimePaymentAssets",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ActAndAssetFileLink",
                table: "OneTimePaymentAssets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActAndAssetNumber",
                table: "OneTimePaymentAssets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AggreementDate",
                table: "OneTimePaymentAssets",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AggreementFileLink",
                table: "OneTimePaymentAssets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AggreementNumber",
                table: "OneTimePaymentAssets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "AmountOfAssetSold",
                table: "OneTimePaymentAssets",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "AmountPayed",
                table: "OneTimePaymentAssets",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "AssetBuyerName",
                table: "OneTimePaymentAssets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Percentage",
                table: "OneTimePaymentAssets",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AlterColumn<int>(
                name: "SolutionFileId",
                table: "InstallmentAssets",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AggreementFileId",
                table: "InstallmentAssets",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActAndAssetDate",
                table: "InstallmentAssets",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ActAndAssetFileId",
                table: "InstallmentAssets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ActAndAssetFileLink",
                table: "InstallmentAssets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActAndAssetNumber",
                table: "InstallmentAssets",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReductionInAssets_FileId",
                table: "ReductionInAssets",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_FileModels_ReductionInAssetId",
                table: "FileModels",
                column: "ReductionInAssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReductionInAssets_FileModels_FileId",
                table: "ReductionInAssets",
                column: "FileId",
                principalTable: "FileModels",
                principalColumn: "FileId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
