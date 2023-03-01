using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace med_game.Migrations
{
    /// <inheritdoc />
    public partial class friend_and_friend_requests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Friend",
                columns: table => new
                {
                    AuthorId = table.Column<long>(type: "bigint", nullable: false),
                    SubscriberId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friend", x => new { x.AuthorId, x.SubscriberId });
                    table.ForeignKey(
                        name: "FK_Friend_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Friend_Users_SubscriberId",
                        column: x => x.SubscriberId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FriendRequest",
                columns: table => new
                {
                    SubscriberId = table.Column<long>(type: "bigint", nullable: false),
                    AuthorId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRequest", x => new { x.AuthorId, x.SubscriberId });
                    table.ForeignKey(
                        name: "FK_FriendRequest_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FriendRequest_Users_SubscriberId",
                        column: x => x.SubscriberId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Friend_SubscriberId",
                table: "Friend",
                column: "SubscriberId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequest_SubscriberId",
                table: "FriendRequest",
                column: "SubscriberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Friend");

            migrationBuilder.DropTable(
                name: "FriendRequest");
        }
    }
}
