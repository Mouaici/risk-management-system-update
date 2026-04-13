using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiskManagement.Migrations
{
    /// <inheritdoc />
    public partial class RenameTablesToSnakeCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActionPlan_incident_incident_id",
                table: "ActionPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_ActionPlan_organization_organization_id",
                table: "ActionPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_ActionPlan_risk_risk_id",
                table: "ActionPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_ActionPlan_user_owner_user_id",
                table: "ActionPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_user_user_id",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActionPlan",
                table: "ActionPlan");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                newName: "refresh_token");

            migrationBuilder.RenameTable(
                name: "ActionPlan",
                newName: "action_plan");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_user_id",
                table: "refresh_token",
                newName: "IX_refresh_token_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_token_hash",
                table: "refresh_token",
                newName: "IX_refresh_token_token_hash");

            migrationBuilder.RenameIndex(
                name: "IX_ActionPlan_risk_id",
                table: "action_plan",
                newName: "IX_action_plan_risk_id");

            migrationBuilder.RenameIndex(
                name: "IX_ActionPlan_owner_user_id",
                table: "action_plan",
                newName: "IX_action_plan_owner_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_ActionPlan_organization_id",
                table: "action_plan",
                newName: "IX_action_plan_organization_id");

            migrationBuilder.RenameIndex(
                name: "IX_ActionPlan_incident_id",
                table: "action_plan",
                newName: "IX_action_plan_incident_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_refresh_token",
                table: "refresh_token",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_action_plan",
                table: "action_plan",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_action_plan_incident_incident_id",
                table: "action_plan",
                column: "incident_id",
                principalTable: "incident",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_action_plan_organization_organization_id",
                table: "action_plan",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_action_plan_risk_risk_id",
                table: "action_plan",
                column: "risk_id",
                principalTable: "risk",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_action_plan_user_owner_user_id",
                table: "action_plan",
                column: "owner_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_token_user_user_id",
                table: "refresh_token",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_action_plan_incident_incident_id",
                table: "action_plan");

            migrationBuilder.DropForeignKey(
                name: "FK_action_plan_organization_organization_id",
                table: "action_plan");

            migrationBuilder.DropForeignKey(
                name: "FK_action_plan_risk_risk_id",
                table: "action_plan");

            migrationBuilder.DropForeignKey(
                name: "FK_action_plan_user_owner_user_id",
                table: "action_plan");

            migrationBuilder.DropForeignKey(
                name: "FK_refresh_token_user_user_id",
                table: "refresh_token");

            migrationBuilder.DropPrimaryKey(
                name: "PK_refresh_token",
                table: "refresh_token");

            migrationBuilder.DropPrimaryKey(
                name: "PK_action_plan",
                table: "action_plan");

            migrationBuilder.RenameTable(
                name: "refresh_token",
                newName: "RefreshTokens");

            migrationBuilder.RenameTable(
                name: "action_plan",
                newName: "ActionPlan");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_token_user_id",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_token_token_hash",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_token_hash");

            migrationBuilder.RenameIndex(
                name: "IX_action_plan_risk_id",
                table: "ActionPlan",
                newName: "IX_ActionPlan_risk_id");

            migrationBuilder.RenameIndex(
                name: "IX_action_plan_owner_user_id",
                table: "ActionPlan",
                newName: "IX_ActionPlan_owner_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_action_plan_organization_id",
                table: "ActionPlan",
                newName: "IX_ActionPlan_organization_id");

            migrationBuilder.RenameIndex(
                name: "IX_action_plan_incident_id",
                table: "ActionPlan",
                newName: "IX_ActionPlan_incident_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActionPlan",
                table: "ActionPlan",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActionPlan_incident_incident_id",
                table: "ActionPlan",
                column: "incident_id",
                principalTable: "incident",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ActionPlan_organization_organization_id",
                table: "ActionPlan",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActionPlan_risk_risk_id",
                table: "ActionPlan",
                column: "risk_id",
                principalTable: "risk",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ActionPlan_user_owner_user_id",
                table: "ActionPlan",
                column: "owner_user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_user_user_id",
                table: "RefreshTokens",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
