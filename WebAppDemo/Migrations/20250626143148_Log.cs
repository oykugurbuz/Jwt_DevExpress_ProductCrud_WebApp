using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppDemo.Migrations
{
    /// <inheritdoc />
    public partial class Log : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RevokedByUserId",
                table: "UserPermissions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RevokedDate",
                table: "UserPermissions",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RevokedByUserId",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "RevokedDate",
                table: "UserPermissions");
        }
    }
}
