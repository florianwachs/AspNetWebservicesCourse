using Microsoft.EntityFrameworkCore.Migrations;

namespace ChuckNorrisService.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Jokes",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    JokeText = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jokes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JokeCategories",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    JokeId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JokeCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JokeCategories_Jokes_JokeId",
                        column: x => x.JokeId,
                        principalTable: "Jokes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JokeCategories_JokeId",
                table: "JokeCategories",
                column: "JokeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JokeCategories");

            migrationBuilder.DropTable(
                name: "Jokes");
        }
    }
}
