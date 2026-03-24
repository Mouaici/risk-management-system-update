using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiskManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddRiskAssetId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "asset_id",
                table: "risk",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_risk_asset_id",
                table: "risk",
                column: "asset_id");

            migrationBuilder.AddForeignKey(
                name: "FK_risk_asset_asset_id",
                table: "risk",
                column: "asset_id",
                principalTable: "asset",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_risk_asset_asset_id",
                table: "risk");

            migrationBuilder.DropIndex(
                name: "IX_risk_asset_id",
                table: "risk");

            migrationBuilder.DropColumn(
                name: "asset_id",
                table: "risk");
        }
    }
}
