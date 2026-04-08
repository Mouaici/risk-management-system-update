using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiskManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddActionPlanTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "action_plan",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    risk_id = table.Column<int>(type: "int", nullable: true),
                    incident_id = table.Column<int>(type: "int", nullable: true),
                    owner_user_id = table.Column<int>(type: "int", nullable: true),
                    suggested_action = table.Column<string>(type: "text", nullable: true),
                    planned_completion_date = table.Column<DateTime>(type: "date", nullable: true),
                    action_plan_status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    follow_up = table.Column<string>(type: "text", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_action_plan", x => x.id);
                    table.ForeignKey(
                        name: "FK_action_plan_risk_risk_id",
                        column: x => x.risk_id,
                        principalTable: "risk",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_action_plan_user_owner_user_id",
                        column: x => x.owner_user_id,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_action_plan_incident_incident_id",
                        column: x => x.incident_id,
                        principalTable: "incident",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_action_plan_risk_id",
                table: "action_plan",
                column: "risk_id");

            migrationBuilder.CreateIndex(
                name: "IX_action_plan_owner_user_id",
                table: "action_plan",
                column: "owner_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_action_plan_incident_id",
                table: "action_plan",
                column: "incident_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "action_plan");
        }
    }
}
