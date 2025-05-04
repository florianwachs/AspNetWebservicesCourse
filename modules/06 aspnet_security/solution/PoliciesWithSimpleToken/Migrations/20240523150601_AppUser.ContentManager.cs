using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PoliciesWithSimpleToken.Migrations
{
    /// <inheritdoc />
    public partial class AppUserContentManager : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsContentManager",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsContentManager",
                table: "AspNetUsers");
        }
    }
}
