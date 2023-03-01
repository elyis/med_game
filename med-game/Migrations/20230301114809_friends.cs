using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace med_game.Migrations
{
    /// <inheritdoc />
    public partial class friends : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friend_Users_AuthorId",
                table: "Friend");

            migrationBuilder.DropForeignKey(
                name: "FK_Friend_Users_SubscriberId",
                table: "Friend");

            migrationBuilder.AddForeignKey(
                name: "FK_Friend_Users_AuthorId",
                table: "Friend",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friend_Users_SubscriberId",
                table: "Friend",
                column: "SubscriberId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friend_Users_AuthorId",
                table: "Friend");

            migrationBuilder.DropForeignKey(
                name: "FK_Friend_Users_SubscriberId",
                table: "Friend");

            migrationBuilder.AddForeignKey(
                name: "FK_Friend_Users_AuthorId",
                table: "Friend",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Friend_Users_SubscriberId",
                table: "Friend",
                column: "SubscriberId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
