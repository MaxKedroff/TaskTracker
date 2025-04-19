using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskTracker.Migrations
{
    /// <inheritdoc />
    public partial class updateRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserRoles_ProjectId",
                table: "UserRoles");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Permissions", "Title" },
                values: new object[,]
                {
                    { 1, true, "Admin" },
                    { 2, false, "Employee" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_ProjectId_UserId",
                table: "UserRoles",
                columns: new[] { "ProjectId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserRoles_ProjectId_UserId",
                table: "UserRoles");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_ProjectId",
                table: "UserRoles",
                column: "ProjectId");
        }
    }
}
