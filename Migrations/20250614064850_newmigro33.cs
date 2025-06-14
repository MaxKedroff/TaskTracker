using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTracker.Migrations
{
    /// <inheritdoc />
    public partial class newmigro33 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Defects");

            migrationBuilder.DropColumn(
                name: "Severity",
                table: "Defects");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Defects");

            migrationBuilder.AddColumn<bool>(
                name: "IsDone",
                table: "Defects",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PriorityId",
                table: "Defects",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Defects_PriorityId",
                table: "Defects",
                column: "PriorityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Defects_Priority_PriorityId",
                table: "Defects",
                column: "PriorityId",
                principalTable: "Priority",
                principalColumn: "PriorityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Defects_Priority_PriorityId",
                table: "Defects");

            migrationBuilder.DropIndex(
                name: "IX_Defects_PriorityId",
                table: "Defects");

            migrationBuilder.DropColumn(
                name: "IsDone",
                table: "Defects");

            migrationBuilder.DropColumn(
                name: "PriorityId",
                table: "Defects");

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Defects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Severity",
                table: "Defects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Defects",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
