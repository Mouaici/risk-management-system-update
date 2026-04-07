using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiskManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddIncidentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_asset_organization_organization_id",
                table: "asset");

            migrationBuilder.DropForeignKey(
                name: "FK_risk_organization_organization_id",
                table: "risk");

            migrationBuilder.DropForeignKey(
                name: "FK_user_organization_organization_id",
                table: "user");

            migrationBuilder.AddForeignKey(
                name: "FK_asset_organization_organization_id",
                table: "asset",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_risk_organization_organization_id",
                table: "risk",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_organization_organization_id",
                table: "user",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_asset_organization_organization_id",
                table: "asset");

            migrationBuilder.DropForeignKey(
                name: "FK_risk_organization_organization_id",
                table: "risk");

            migrationBuilder.DropForeignKey(
                name: "FK_user_organization_organization_id",
                table: "user");

            migrationBuilder.AddForeignKey(
                name: "FK_asset_organization_organization_id",
                table: "asset",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_risk_organization_organization_id",
                table: "risk",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_user_organization_organization_id",
                table: "user",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
