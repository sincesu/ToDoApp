using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentTableinToDoItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ToDoItemsid",
                table: "Comment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ToDoItemsid",
                table: "Comment",
                column: "ToDoItemsid");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_ToDoItems_ToDoItemsid",
                table: "Comment",
                column: "ToDoItemsid",
                principalTable: "ToDoItems",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_ToDoItems_ToDoItemsid",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_ToDoItemsid",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "ToDoItemsid",
                table: "Comment");
        }
    }
}
