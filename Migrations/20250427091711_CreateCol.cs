using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTracker.Migrations
{
    /// <inheritdoc />
    public partial class CreateCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Defects_Boards_BoardId",
                table: "Defects");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Boards_BoardId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "BoardId",
                table: "Tasks",
                newName: "ColumnId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_BoardId",
                table: "Tasks",
                newName: "IX_Tasks_ColumnId");

            migrationBuilder.RenameColumn(
                name: "BoardId",
                table: "Defects",
                newName: "ColumnId");

            migrationBuilder.RenameIndex(
                name: "IX_Defects_BoardId",
                table: "Defects",
                newName: "IX_Defects_ColumnId");

            migrationBuilder.CreateTable(
                name: "Column",
                columns: table => new
                {
                    ColumnID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BoardId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Color = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Column", x => x.ColumnID);
                    table.ForeignKey(
                        name: "FK_Column_Boards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "Boards",
                        principalColumn: "BoardId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Column_BoardId",
                table: "Column",
                column: "BoardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Defects_Column_ColumnId",
                table: "Defects",
                column: "ColumnId",
                principalTable: "Column",
                principalColumn: "ColumnID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Column_ColumnId",
                table: "Tasks",
                column: "ColumnId",
                principalTable: "Column",
                principalColumn: "ColumnID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Defects_Column_ColumnId",
                table: "Defects");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Column_ColumnId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "Column");

            migrationBuilder.RenameColumn(
                name: "ColumnId",
                table: "Tasks",
                newName: "BoardId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_ColumnId",
                table: "Tasks",
                newName: "IX_Tasks_BoardId");

            migrationBuilder.RenameColumn(
                name: "ColumnId",
                table: "Defects",
                newName: "BoardId");

            migrationBuilder.RenameIndex(
                name: "IX_Defects_ColumnId",
                table: "Defects",
                newName: "IX_Defects_BoardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Defects_Boards_BoardId",
                table: "Defects",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "BoardId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Boards_BoardId",
                table: "Tasks",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "BoardId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
