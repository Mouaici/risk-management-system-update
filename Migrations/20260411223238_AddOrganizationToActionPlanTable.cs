using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiskManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationToActionPlanTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActionPlan_organization_OrganizationId",
                table: "ActionPlan");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "ActionPlan",
                newName: "organization_id");

            migrationBuilder.RenameIndex(
                name: "IX_ActionPlan_OrganizationId",
                table: "ActionPlan",
                newName: "IX_ActionPlan_organization_id");

            migrationBuilder.AlterColumn<int>(
                name: "organization_id",
                table: "ActionPlan",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ActionPlan_organization_organization_id",
                table: "ActionPlan",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActionPlan_organization_organization_id",
                table: "ActionPlan");

            migrationBuilder.RenameColumn(
                name: "organization_id",
                table: "ActionPlan",
                newName: "OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_ActionPlan_organization_id",
                table: "ActionPlan",
                newName: "IX_ActionPlan_OrganizationId");

            migrationBuilder.AlterColumn<int>(
                name: "OrganizationId",
                table: "ActionPlan",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ActionPlan_organization_OrganizationId",
                table: "ActionPlan",
                column: "OrganizationId",
                principalTable: "organization",
                principalColumn: "id");
        }
    }
}
