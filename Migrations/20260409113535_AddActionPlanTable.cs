using System;
using Microsoft.EntityFrameworkCore.Metadata;
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
                name: "organization",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    organization_name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    iso_scope = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    organization_status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organization", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "asset",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    organization_id = table.Column<int>(type: "int", nullable: false),
                    asset_name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    asset_definition = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    asset_type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    classification = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    asset_status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asset", x => x.id);
                    table.ForeignKey(
                        name: "FK_asset_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    first_name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_role = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    organization_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "incident",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    organization_id = table.Column<int>(type: "int", nullable: true),
                    reported_by_user_id = table.Column<int>(type: "int", nullable: true),
                    title = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    severity = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    occured_on = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    incident_status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_incident", x => x.id);
                    table.ForeignKey(
                        name: "FK_incident_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_incident_user_reported_by_user_id",
                        column: x => x.reported_by_user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    token_hash = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    revoked_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    replaced_by_token_hash = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by_ip = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    revoked_by_ip = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "risk",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    organization_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    risk_definition = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    risk_owner_user_id = table.Column<int>(type: "int", nullable: true),
                    asset_id = table.Column<int>(type: "int", nullable: true),
                    risk_status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    likelihood = table.Column<int>(type: "int", nullable: false),
                    impact = table.Column<int>(type: "int", nullable: false),
                    score = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_risk", x => x.id);
                    table.ForeignKey(
                        name: "FK_risk_asset_asset_id",
                        column: x => x.asset_id,
                        principalTable: "asset",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_risk_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_risk_user_risk_owner_user_id",
                        column: x => x.risk_owner_user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_profile_change_request",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    organization_id = table.Column<int>(type: "int", nullable: false),
                    requested_first_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    requested_last_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    requested_email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    request_status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    reviewed_by_user_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    reviewed_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_profile_change_request", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_profile_change_request_user_reviewed_by_user_id",
                        column: x => x.reviewed_by_user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_user_profile_change_request_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ActionPlan",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    risk_id = table.Column<int>(type: "int", nullable: true),
                    incident_id = table.Column<int>(type: "int", nullable: true),
                    owner_user_id = table.Column<int>(type: "int", nullable: true),
                    suggested_action = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    planned_completion_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    action_plan_status = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    follow_up = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    OrganizationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionPlan", x => x.id);
                    table.ForeignKey(
                        name: "FK_ActionPlan_incident_incident_id",
                        column: x => x.incident_id,
                        principalTable: "incident",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ActionPlan_organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organization",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_ActionPlan_risk_risk_id",
                        column: x => x.risk_id,
                        principalTable: "risk",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ActionPlan_user_owner_user_id",
                        column: x => x.owner_user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ActionPlan_incident_id",
                table: "ActionPlan",
                column: "incident_id");

            migrationBuilder.CreateIndex(
                name: "IX_ActionPlan_OrganizationId",
                table: "ActionPlan",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionPlan_owner_user_id",
                table: "ActionPlan",
                column: "owner_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ActionPlan_risk_id",
                table: "ActionPlan",
                column: "risk_id");

            migrationBuilder.CreateIndex(
                name: "IX_asset_organization_id",
                table: "asset",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "IX_incident_organization_id",
                table: "incident",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "IX_incident_reported_by_user_id",
                table: "incident",
                column: "reported_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_token_hash",
                table: "RefreshTokens",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_user_id",
                table: "RefreshTokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_risk_asset_id",
                table: "risk",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_risk_organization_id",
                table: "risk",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "IX_risk_risk_owner_user_id",
                table: "risk",
                column: "risk_owner_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_email",
                table: "user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_organization_id",
                table: "user",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_profile_change_request_reviewed_by_user_id",
                table: "user_profile_change_request",
                column: "reviewed_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_profile_change_request_user_id",
                table: "user_profile_change_request",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionPlan");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "user_profile_change_request");

            migrationBuilder.DropTable(
                name: "incident");

            migrationBuilder.DropTable(
                name: "risk");

            migrationBuilder.DropTable(
                name: "asset");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "organization");
        }
    }
}
