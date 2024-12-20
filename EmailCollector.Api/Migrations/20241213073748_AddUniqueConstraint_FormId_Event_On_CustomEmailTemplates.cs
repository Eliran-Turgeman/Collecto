using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailCollector.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraint_FormId_Event_On_CustomEmailTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomEmailTemplates_FormId",
                table: "CustomEmailTemplates");

            migrationBuilder.CreateIndex(
                name: "IX_CustomEmailTemplate_FormId_Event_Unique",
                table: "CustomEmailTemplates",
                columns: new[] { "FormId", "Event" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomEmailTemplate_FormId_Event_Unique",
                table: "CustomEmailTemplates");

            migrationBuilder.CreateIndex(
                name: "IX_CustomEmailTemplates_FormId",
                table: "CustomEmailTemplates",
                column: "FormId");
        }
    }
}
