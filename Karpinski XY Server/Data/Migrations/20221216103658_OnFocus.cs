using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Karpinski_XY_Server.Migrations
{
    public partial class OnFocus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OnFocus",
                table: "Paintings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnFocus",
                table: "Paintings");
        }
    }
}
