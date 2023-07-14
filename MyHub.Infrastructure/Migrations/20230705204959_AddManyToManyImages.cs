using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddManyToManyImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersGallery_Users_UserId",
                table: "UsersGallery");

            migrationBuilder.DropIndex(
                name: "IX_UsersGallery_UserId",
                table: "UsersGallery");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UsersGallery");

            migrationBuilder.CreateTable(
                name: "GalleryImageUser",
                columns: table => new
                {
                    GalleryImagesId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LikedUsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GalleryImageUser", x => new { x.GalleryImagesId, x.LikedUsersId });
                    table.ForeignKey(
                        name: "FK_GalleryImageUser_UsersGallery_GalleryImagesId",
                        column: x => x.GalleryImagesId,
                        principalTable: "UsersGallery",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GalleryImageUser_Users_LikedUsersId",
                        column: x => x.LikedUsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GalleryImageUser_LikedUsersId",
                table: "GalleryImageUser",
                column: "LikedUsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GalleryImageUser");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "UsersGallery",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UsersGallery_UserId",
                table: "UsersGallery",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersGallery_Users_UserId",
                table: "UsersGallery",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
