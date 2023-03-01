using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace med_game.Migrations
{
    /// <inheritdoc />
    public partial class user_token_and_role : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoleName",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "TokenValidBefore",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TokenValidBefore",
                table: "Users");
        }
    }
}
