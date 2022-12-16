using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Karpinski_XY_Server.Migrations
{
    public partial class AdditionalProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                table: "Paintings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Technique",
                table: "Paintings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Paintings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortDescription",
                table: "Paintings");

            migrationBuilder.DropColumn(
                name: "Technique",
                table: "Paintings");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Paintings");
        }
    }
}
