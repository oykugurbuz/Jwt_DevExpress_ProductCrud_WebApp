using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppDemo.Migrations
{
    /// <inheritdoc />
    public partial class RevokedByUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_RevokedByUserId",
                table: "UserPermissions",
                column: "RevokedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissions_AppUserInfos_RevokedByUserId",
                table: "UserPermissions",
                column: "RevokedByUserId",
                principalTable: "AppUserInfos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissions_AppUserInfos_RevokedByUserId",
                table: "UserPermissions");

            migrationBuilder.DropIndex(
                name: "IX_UserPermissions_RevokedByUserId",
                table: "UserPermissions");
        }
    }
}
