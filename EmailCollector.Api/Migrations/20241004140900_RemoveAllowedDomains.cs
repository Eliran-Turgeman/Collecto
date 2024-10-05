using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailCollector.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAllowedDomains : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailSignups_SignupForms_SignupFormId",
                table: "EmailSignups");

            migrationBuilder.DropIndex(
                name: "IX_EmailSignups_SignupFormId",
                table: "EmailSignups");

            migrationBuilder.DropColumn(
                name: "AllowedDomains",
                table: "SignupForms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AllowedDomains",
                table: "SignupForms",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_EmailSignups_SignupFormId",
                table: "EmailSignups",
                column: "SignupFormId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailSignups_SignupForms_SignupFormId",
                table: "EmailSignups",
                column: "SignupFormId",
                principalTable: "SignupForms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
