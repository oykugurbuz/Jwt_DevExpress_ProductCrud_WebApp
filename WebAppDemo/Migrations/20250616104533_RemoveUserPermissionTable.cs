using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppDemo.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserPermissionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPermissions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPermissions",
                columns: table => new
                {
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CanCategoryAdd = table.Column<bool>(type: "bit", nullable: false),
                    CanCategoryDelete = table.Column<bool>(type: "bit", nullable: false),
                    CanCategoryRead = table.Column<bool>(type: "bit", nullable: false),
                    CanCategoryUpdate = table.Column<bool>(type: "bit", nullable: false),
                    CanProductAdd = table.Column<bool>(type: "bit", nullable: false),
                    CanProductDelete = table.Column<bool>(type: "bit", nullable: false),
                    CanProductRead = table.Column<bool>(type: "bit", nullable: false),
                    CanProductUpdate = table.Column<bool>(type: "bit", nullable: false),
                    CanReport = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => x.PermissionId);
                    table.ForeignKey(
                        name: "FK_UserPermissions_AppUserInfos_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUserInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_UserId",
                table: "UserPermissions",
                column: "UserId");
        }
    }
}
