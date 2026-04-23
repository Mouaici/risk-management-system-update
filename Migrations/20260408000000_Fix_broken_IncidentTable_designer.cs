using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiskManagement.Migrations
{
    /// <inheritdoc />
    public partial class Fix_broken_IncidentTable_designer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_incident_organization_id",
                table: "incident",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "IX_incident_reported_by_user_id",
                table: "incident",
                column: "reported_by_user_id");
        }

        

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
               name: "incident");
        }
    }
}
