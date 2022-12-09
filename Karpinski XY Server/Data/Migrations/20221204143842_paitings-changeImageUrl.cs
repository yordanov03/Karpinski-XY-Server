using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Karpinski_XY_Server.Migrations
{
    public partial class paitingschangeImageUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Dimensions",
                table: "Paintings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dimensions",
                table: "Paintings");
        }
    }
}
