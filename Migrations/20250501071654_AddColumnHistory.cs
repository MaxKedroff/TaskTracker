using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ColumnHistories",
                columns: table => new
                {
                    ColumnHistoryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TaskId = table.Column<int>(type: "INTEGER", nullable: false),
                    OldColumnId = table.Column<int>(type: "INTEGER", nullable: true),
                    NewColumnId = table.Column<int>(type: "INTEGER", nullable: true),
                    ChangeDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColumnHistories", x => x.ColumnHistoryId);
                    table.ForeignKey(
                        name: "FK_ColumnHistories_Columns_NewColumnId",
                        column: x => x.NewColumnId,
                        principalTable: "Columns",
                        principalColumn: "ColumnID");
                    table.ForeignKey(
                        name: "FK_ColumnHistories_Columns_OldColumnId",
                        column: x => x.OldColumnId,
                        principalTable: "Columns",
                        principalColumn: "ColumnID");
                    table.ForeignKey(
                        name: "FK_ColumnHistories_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "TaskId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ColumnHistories_NewColumnId",
                table: "ColumnHistories",
                column: "NewColumnId");

            migrationBuilder.CreateIndex(
                name: "IX_ColumnHistories_OldColumnId",
                table: "ColumnHistories",
                column: "OldColumnId");

            migrationBuilder.CreateIndex(
                name: "IX_ColumnHistories_TaskId",
                table: "ColumnHistories",
                column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColumnHistories");
        }
    }
}
