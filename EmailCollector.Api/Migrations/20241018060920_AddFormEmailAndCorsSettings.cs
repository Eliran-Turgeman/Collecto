using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailCollector.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFormEmailAndCorsSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormCorsSettings",
                columns: table => new
                {
                    FormId = table.Column<int>(type: "INTEGER", nullable: false),
                    AllowedOrigins = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormCorsSettings", x => x.FormId);
                    table.ForeignKey(
                        name: "FK_FormCorsSettings_SignupForms_FormId",
                        column: x => x.FormId,
                        principalTable: "SignupForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormEmailSettings",
                columns: table => new
                {
                    FormId = table.Column<int>(type: "INTEGER", nullable: false),
                    EmailMethod = table.Column<string>(type: "TEXT", nullable: false),
                    EmailFrom = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormEmailSettings", x => x.FormId);
                    table.ForeignKey(
                        name: "FK_FormEmailSettings_SignupForms_FormId",
                        column: x => x.FormId,
                        principalTable: "SignupForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmtpEmailSettings",
                columns: table => new
                {
                    FormId = table.Column<int>(type: "INTEGER", nullable: false),
                    SmtpServer = table.Column<string>(type: "TEXT", nullable: false),
                    SmtpPort = table.Column<int>(type: "INTEGER", nullable: false),
                    SmtpUsername = table.Column<string>(type: "TEXT", nullable: false),
                    SmtpPassword = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmtpEmailSettings", x => x.FormId);
                    table.ForeignKey(
                        name: "FK_SmtpEmailSettings_FormEmailSettings_FormId",
                        column: x => x.FormId,
                        principalTable: "FormEmailSettings",
                        principalColumn: "FormId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormCorsSettings");

            migrationBuilder.DropTable(
                name: "SmtpEmailSettings");

            migrationBuilder.DropTable(
                name: "FormEmailSettings");
        }
    }
}
