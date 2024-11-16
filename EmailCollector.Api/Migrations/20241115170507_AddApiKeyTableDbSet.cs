using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailCollector.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddApiKeyTableDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiKey_AspNetUsers_UserId",
                table: "ApiKey");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiKey",
                table: "ApiKey");

            migrationBuilder.RenameTable(
                name: "ApiKey",
                newName: "ApiKeys");

            migrationBuilder.RenameIndex(
                name: "IX_ApiKey_UserId",
                table: "ApiKeys",
                newName: "IX_ApiKeys_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiKeys",
                table: "ApiKeys",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiKeys_AspNetUsers_UserId",
                table: "ApiKeys",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiKeys_AspNetUsers_UserId",
                table: "ApiKeys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiKeys",
                table: "ApiKeys");

            migrationBuilder.RenameTable(
                name: "ApiKeys",
                newName: "ApiKey");

            migrationBuilder.RenameIndex(
                name: "IX_ApiKeys_UserId",
                table: "ApiKey",
                newName: "IX_ApiKey_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiKey",
                table: "ApiKey",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiKey_AspNetUsers_UserId",
                table: "ApiKey",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
