using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImageComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GalleryImageComment",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ImageId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GalleryImageComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GalleryImageComment_GalleryImages_ImageId",
                        column: x => x.ImageId,
                        principalTable: "GalleryImages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GalleryImageComment_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GalleryImageComment_ImageId",
                table: "GalleryImageComment",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_GalleryImageComment_UserId",
                table: "GalleryImageComment",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GalleryImageComment");
        }
    }
}
