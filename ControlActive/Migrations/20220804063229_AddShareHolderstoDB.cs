using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ControlActive.Migrations
{
    public partial class AddShareHolderstoDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountFromAuthCapital",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "ShareHolderName",
                table: "Shares");

            migrationBuilder.CreateTable(
                name: "Shareholders",
                columns: table => new
                {
                    ShareholderId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShareholderName = table.Column<string>(type: "text", nullable: true),
                    AmountFromAuthCapital = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shareholders", x => x.ShareholderId);
                });

            migrationBuilder.CreateTable(
                name: "SharesAndHolders",
                columns: table => new
                {
                    ShareId = table.Column<int>(type: "integer", nullable: false),
                    ShareholderId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharesAndHolders", x => new { x.ShareId, x.ShareholderId });
                    table.ForeignKey(
                        name: "FK_SharesAndHolders_Shareholders_ShareholderId",
                        column: x => x.ShareholderId,
                        principalTable: "Shareholders",
                        principalColumn: "ShareholderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SharesAndHolders_Shares_ShareId",
                        column: x => x.ShareId,
                        principalTable: "Shares",
                        principalColumn: "ShareId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShareShareholder",
                columns: table => new
                {
                    ShareholdersShareholderId = table.Column<int>(type: "integer", nullable: false),
                    SharesShareId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShareShareholder", x => new { x.ShareholdersShareholderId, x.SharesShareId });
                    table.ForeignKey(
                        name: "FK_ShareShareholder_Shareholders_ShareholdersShareholderId",
                        column: x => x.ShareholdersShareholderId,
                        principalTable: "Shareholders",
                        principalColumn: "ShareholderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShareShareholder_Shares_SharesShareId",
                        column: x => x.SharesShareId,
                        principalTable: "Shares",
                        principalColumn: "ShareId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SharesAndHolders_ShareholderId",
                table: "SharesAndHolders",
                column: "ShareholderId");

            migrationBuilder.CreateIndex(
                name: "IX_ShareShareholder_SharesShareId",
                table: "ShareShareholder",
                column: "SharesShareId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SharesAndHolders");

            migrationBuilder.DropTable(
                name: "ShareShareholder");

            migrationBuilder.DropTable(
                name: "Shareholders");

            migrationBuilder.AddColumn<string>(
                name: "AmountFromAuthCapital",
                table: "Shares",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShareHolderName",
                table: "Shares",
                type: "text",
                nullable: true);
        }
    }
}
