using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppDemo.Migrations
{
    /// <inheritdoc />
    public partial class AddRevokedByUserRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissions_AppUserInfos_RevokedByUserId",
                table: "UserPermissions");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissions_AppUserInfos_RevokedByUserId",
                table: "UserPermissions",
                column: "RevokedByUserId",
                principalTable: "AppUserInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissions_AppUserInfos_RevokedByUserId",
                table: "UserPermissions");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissions_AppUserInfos_RevokedByUserId",
                table: "UserPermissions",
                column: "RevokedByUserId",
                principalTable: "AppUserInfos",
                principalColumn: "Id");
        }
    }
}
