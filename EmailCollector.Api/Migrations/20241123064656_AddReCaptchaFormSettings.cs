using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailCollector.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddReCaptchaFormSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecaptchaFormSettings",
                columns: table => new
                {
                    FormId = table.Column<int>(type: "INTEGER", nullable: false),
                    SiteKey = table.Column<string>(type: "TEXT", nullable: false),
                    SecretKey = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecaptchaFormSettings", x => x.FormId);
                    table.ForeignKey(
                        name: "FK_RecaptchaFormSettings_SignupForms_FormId",
                        column: x => x.FormId,
                        principalTable: "SignupForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecaptchaFormSettings");
        }
    }
}
