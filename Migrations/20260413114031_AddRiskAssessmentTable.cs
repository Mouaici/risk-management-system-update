using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiskManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddRiskAssessmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "risk_assessment",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    risk_id = table.Column<int>(type: "int", nullable: true),
                    assessed_by_user_id = table.Column<int>(type: "int", nullable: true),
                    organization_id = table.Column<int>(type: "int", nullable: false),
                    notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    risk_phase = table.Column<string>(type: "varchar(100)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    likelihood_1_5 = table.Column<int>(type: "int", nullable: true),
                    impact_1_5 = table.Column<int>(type: "int", nullable: true),
                    risk_score = table.Column<int>(type: "int", nullable: true),
                    economical_loss = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    risk_mitigation = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    risk_transformation = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    risk_avoidance = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    risk_acceptance = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_risk_assessment", x => x.id);
                    table.ForeignKey(
                        name: "FK_risk_assessment_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_risk_assessment_risk_risk_id",
                        column: x => x.risk_id,
                        principalTable: "risk",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_risk_assessment_user_assessed_by_user_id",
                        column: x => x.assessed_by_user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_risk_assessment_assessed_by_user_id",
                table: "risk_assessment",
                column: "assessed_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_risk_assessment_organization_id",
                table: "risk_assessment",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "IX_risk_assessment_risk_id",
                table: "risk_assessment",
                column: "risk_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "risk_assessment");
        }
    }
}
