using Microsoft.EntityFrameworkCore.Migrations;

namespace ReactAppWithAuth1.Data.Migrations
{
    public partial class CustomPropertyIsPayingCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPayingCustomer",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPayingCustomer",
                table: "AspNetUsers");
        }
    }
}
