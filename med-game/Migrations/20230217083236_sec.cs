using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace med_game.Migrations
{
    /// <inheritdoc />
    public partial class sec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "maxCountPoints",
                table: "Achievements",
                newName: "MaxCountPoints");

            migrationBuilder.RenameColumn(
                name: "countPoints",
                table: "Achievements",
                newName: "CountPoints");

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Image = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lecterns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lecterns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    LecternId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modules_Lecterns_LecternId",
                        column: x => x.LecternId,
                        principalTable: "Lecterns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Image = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: false),
                    TimeSeconds = table.Column<int>(type: "integer", nullable: false),
                    CountPointsPerAnswer = table.Column<int>(type: "integer", nullable: false),
                    CorrectAnswerIndex = table.Column<long>(type: "bigint", nullable: false),
                    ModuleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnswerQuestion",
                columns: table => new
                {
                    AnswersId = table.Column<long>(type: "bigint", nullable: false),
                    QuestionsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerQuestion", x => new { x.AnswersId, x.QuestionsId });
                    table.ForeignKey(
                        name: "FK_AnswerQuestion_Answers_AnswersId",
                        column: x => x.AnswersId,
                        principalTable: "Answers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnswerQuestion_Questions_QuestionsId",
                        column: x => x.QuestionsId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerQuestion_QuestionsId",
                table: "AnswerQuestion",
                column: "QuestionsId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_Type",
                table: "Answers",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Lecterns_Name",
                table: "Lecterns",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Modules_LecternId",
                table: "Modules",
                column: "LecternId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ModuleId",
                table: "Questions",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_Type",
                table: "Questions",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerQuestion");

            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "Lecterns");

            migrationBuilder.RenameColumn(
                name: "MaxCountPoints",
                table: "Achievements",
                newName: "maxCountPoints");

            migrationBuilder.RenameColumn(
                name: "CountPoints",
                table: "Achievements",
                newName: "countPoints");
        }
    }
}
