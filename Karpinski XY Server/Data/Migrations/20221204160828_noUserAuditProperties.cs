using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Karpinski_XY_Server.Migrations
{
    public partial class noUserAuditProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Paintings");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Paintings");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Paintings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Paintings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Paintings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Paintings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
