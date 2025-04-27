using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTracker.Migrations
{
    /// <inheritdoc />
    public partial class newMigro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Backlogs_TaskId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_UserRoles_TaskId",
                table: "Tasks");

            migrationBuilder.AlterColumn<int>(
                name: "TaskId",
                table: "Tasks",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "BacklogId",
                table: "Tasks",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_BacklogId",
                table: "Tasks",
                column: "BacklogId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserRoleId",
                table: "Tasks",
                column: "UserRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Backlogs_BacklogId",
                table: "Tasks",
                column: "BacklogId",
                principalTable: "Backlogs",
                principalColumn: "BacklogId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_UserRoles_UserRoleId",
                table: "Tasks",
                column: "UserRoleId",
                principalTable: "UserRoles",
                principalColumn: "UserRoleId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Backlogs_BacklogId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_UserRoles_UserRoleId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_BacklogId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_UserRoleId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "BacklogId",
                table: "Tasks");

            migrationBuilder.AlterColumn<int>(
                name: "TaskId",
                table: "Tasks",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Backlogs_TaskId",
                table: "Tasks",
                column: "TaskId",
                principalTable: "Backlogs",
                principalColumn: "BacklogId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_UserRoles_TaskId",
                table: "Tasks",
                column: "TaskId",
                principalTable: "UserRoles",
                principalColumn: "UserRoleId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
