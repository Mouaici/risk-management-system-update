using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiskManagement.Migrations
{
    /// <inheritdoc />
    public partial class FixRiskAssessmentRequiredAndRenameTransfer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_risk_assessment_risk_risk_id",
                table: "risk_assessment");

            migrationBuilder.DropForeignKey(
                name: "FK_risk_assessment_user_assessed_by_user_id",
                table: "risk_assessment");

            migrationBuilder.RenameColumn(
                name: "risk_transformation",
                table: "risk_assessment",
                newName: "risk_transfer");

            migrationBuilder.AlterColumn<int>(
                name: "risk_id",
                table: "risk_assessment",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "assessed_by_user_id",
                table: "risk_assessment",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_risk_assessment_risk_risk_id",
                table: "risk_assessment",
                column: "risk_id",
                principalTable: "risk",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_risk_assessment_user_assessed_by_user_id",
                table: "risk_assessment",
                column: "assessed_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_risk_assessment_risk_risk_id",
                table: "risk_assessment");

            migrationBuilder.DropForeignKey(
                name: "FK_risk_assessment_user_assessed_by_user_id",
                table: "risk_assessment");

            migrationBuilder.RenameColumn(
                name: "risk_transfer",
                table: "risk_assessment",
                newName: "risk_transformation");

            migrationBuilder.AlterColumn<int>(
                name: "risk_id",
                table: "risk_assessment",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "assessed_by_user_id",
                table: "risk_assessment",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_risk_assessment_risk_risk_id",
                table: "risk_assessment",
                column: "risk_id",
                principalTable: "risk",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_risk_assessment_user_assessed_by_user_id",
                table: "risk_assessment",
                column: "assessed_by_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
