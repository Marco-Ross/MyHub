using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCorrectRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GalleryImageUser_UsersGallery_GalleryImagesId",
                table: "GalleryImageUser");

            migrationBuilder.RenameColumn(
                name: "GalleryImagesId",
                table: "GalleryImageUser",
                newName: "LikedImagesId");

            migrationBuilder.AddColumn<string>(
                name: "UserCreatedId",
                table: "UsersGallery",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersGallery_UserCreatedId",
                table: "UsersGallery",
                column: "UserCreatedId");

            migrationBuilder.AddForeignKey(
                name: "FK_GalleryImageUser_UsersGallery_LikedImagesId",
                table: "GalleryImageUser",
                column: "LikedImagesId",
                principalTable: "UsersGallery",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersGallery_Users_UserCreatedId",
                table: "UsersGallery",
                column: "UserCreatedId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GalleryImageUser_UsersGallery_LikedImagesId",
                table: "GalleryImageUser");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersGallery_Users_UserCreatedId",
                table: "UsersGallery");

            migrationBuilder.DropIndex(
                name: "IX_UsersGallery_UserCreatedId",
                table: "UsersGallery");

            migrationBuilder.DropColumn(
                name: "UserCreatedId",
                table: "UsersGallery");

            migrationBuilder.RenameColumn(
                name: "LikedImagesId",
                table: "GalleryImageUser",
                newName: "GalleryImagesId");

            migrationBuilder.AddForeignKey(
                name: "FK_GalleryImageUser_UsersGallery_GalleryImagesId",
                table: "GalleryImageUser",
                column: "GalleryImagesId",
                principalTable: "UsersGallery",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
