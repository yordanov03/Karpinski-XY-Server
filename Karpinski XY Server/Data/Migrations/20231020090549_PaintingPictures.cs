using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Karpinski_XY_Server.Migrations
{
    public partial class PaintingPictures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Paintings");

            migrationBuilder.CreateTable(
                name: "PaintingPictures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsMainPicture = table.Column<bool>(type: "bit", nullable: false),
                    PaintingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaintingPictures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaintingPictures_Paintings_PaintingId",
                        column: x => x.PaintingId,
                        principalTable: "Paintings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaintingPictures_PaintingId",
                table: "PaintingPictures",
                column: "PaintingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaintingPictures");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Paintings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
