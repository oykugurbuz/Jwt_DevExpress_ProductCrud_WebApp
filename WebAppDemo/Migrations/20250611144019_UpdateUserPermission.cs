using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppDemo.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CanUpdate",
                table: "UserPermissions",
                newName: "CanProductUpdate");

            migrationBuilder.RenameColumn(
                name: "CanRead",
                table: "UserPermissions",
                newName: "CanProductRead");

            migrationBuilder.RenameColumn(
                name: "CanDelete",
                table: "UserPermissions",
                newName: "CanProductDelete");

            migrationBuilder.RenameColumn(
                name: "CanAdd",
                table: "UserPermissions",
                newName: "CanProductAdd");

            migrationBuilder.AddColumn<bool>(
                name: "CanCategoryAdd",
                table: "UserPermissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanCategoryDelete",
                table: "UserPermissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanCategoryRead",
                table: "UserPermissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanCategoryUpdate",
                table: "UserPermissions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanCategoryAdd",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "CanCategoryDelete",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "CanCategoryRead",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "CanCategoryUpdate",
                table: "UserPermissions");

            migrationBuilder.RenameColumn(
                name: "CanProductUpdate",
                table: "UserPermissions",
                newName: "CanUpdate");

            migrationBuilder.RenameColumn(
                name: "CanProductRead",
                table: "UserPermissions",
                newName: "CanRead");

            migrationBuilder.RenameColumn(
                name: "CanProductDelete",
                table: "UserPermissions",
                newName: "CanDelete");

            migrationBuilder.RenameColumn(
                name: "CanProductAdd",
                table: "UserPermissions",
                newName: "CanAdd");
        }
    }
}
