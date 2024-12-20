using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailCollector.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomEmailTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomEmailTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FormId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Event = table.Column<string>(type: "TEXT", nullable: false),
                    TemplateSubject = table.Column<string>(type: "TEXT", nullable: false),
                    TemplateBodyUri = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomEmailTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomEmailTemplates_SignupForms_FormId",
                        column: x => x.FormId,
                        principalTable: "SignupForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomEmailTemplates_FormId",
                table: "CustomEmailTemplates",
                column: "FormId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomEmailTemplates");
        }
    }
}
