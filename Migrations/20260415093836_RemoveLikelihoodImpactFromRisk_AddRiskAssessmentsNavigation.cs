using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiskManagement.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLikelihoodImpactFromRisk_AddRiskAssessmentsNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_risk_assessment_organization_organization_id",
                table: "risk_assessment");

            migrationBuilder.DropColumn(
                name: "impact",
                table: "risk");

            migrationBuilder.DropColumn(
                name: "likelihood",
                table: "risk");

            migrationBuilder.DropColumn(
                name: "score",
                table: "risk");

            migrationBuilder.AddForeignKey(
                name: "FK_risk_assessment_organization_organization_id",
                table: "risk_assessment",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_risk_assessment_organization_organization_id",
                table: "risk_assessment");

            migrationBuilder.AddColumn<int>(
                name: "impact",
                table: "risk",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "likelihood",
                table: "risk",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "score",
                table: "risk",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_risk_assessment_organization_organization_id",
                table: "risk_assessment",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
