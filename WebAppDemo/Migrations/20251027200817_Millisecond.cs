using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppDemo.Migrations
{
    /// <inheritdoc />
    public partial class Millisecond : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DurationSeconds",
                table: "ActivityLogs",
                newName: "DurationMs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DurationMs",
                table: "ActivityLogs",
                newName: "DurationSeconds");
        }
    }
}
