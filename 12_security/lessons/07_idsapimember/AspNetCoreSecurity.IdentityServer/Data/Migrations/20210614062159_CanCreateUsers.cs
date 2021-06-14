using Microsoft.EntityFrameworkCore.Migrations;

namespace StsServerIdentity.Data.Migrations
{
    public partial class CanCreateUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanCreateUsers",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanCreateUsers",
                table: "AspNetUsers");
        }
    }
}
