using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTracker.Migrations
{
    /// <inheritdoc />
    public partial class CreateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Column_Boards_BoardId",
                table: "Column");

            migrationBuilder.DropForeignKey(
                name: "FK_Defects_Column_ColumnId",
                table: "Defects");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Column_ColumnId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Column",
                table: "Column");

            migrationBuilder.RenameTable(
                name: "Column",
                newName: "Columns");

            migrationBuilder.RenameIndex(
                name: "IX_Column_BoardId",
                table: "Columns",
                newName: "IX_Columns_BoardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Columns",
                table: "Columns",
                column: "ColumnID");

            migrationBuilder.AddForeignKey(
                name: "FK_Columns_Boards_BoardId",
                table: "Columns",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "BoardId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Defects_Columns_ColumnId",
                table: "Defects",
                column: "ColumnId",
                principalTable: "Columns",
                principalColumn: "ColumnID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Columns_ColumnId",
                table: "Tasks",
                column: "ColumnId",
                principalTable: "Columns",
                principalColumn: "ColumnID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Columns_Boards_BoardId",
                table: "Columns");

            migrationBuilder.DropForeignKey(
                name: "FK_Defects_Columns_ColumnId",
                table: "Defects");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Columns_ColumnId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Columns",
                table: "Columns");

            migrationBuilder.RenameTable(
                name: "Columns",
                newName: "Column");

            migrationBuilder.RenameIndex(
                name: "IX_Columns_BoardId",
                table: "Column",
                newName: "IX_Column_BoardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Column",
                table: "Column",
                column: "ColumnID");

            migrationBuilder.AddForeignKey(
                name: "FK_Column_Boards_BoardId",
                table: "Column",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "BoardId",
                onDelete: ReferentialAction.Cascade);

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
    }
}
