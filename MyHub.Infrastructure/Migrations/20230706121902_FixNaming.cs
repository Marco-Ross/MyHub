using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GalleryImageUser_UsersGallery_LikedImagesId",
                table: "GalleryImageUser");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersGallery_Users_UserCreatedId",
                table: "UsersGallery");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersGallery",
                table: "UsersGallery");

            migrationBuilder.RenameTable(
                name: "UsersGallery",
                newName: "GalleryImages");

            migrationBuilder.RenameIndex(
                name: "IX_UsersGallery_UserCreatedId",
                table: "GalleryImages",
                newName: "IX_GalleryImages_UserCreatedId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GalleryImages",
                table: "GalleryImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GalleryImages_Users_UserCreatedId",
                table: "GalleryImages",
                column: "UserCreatedId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GalleryImageUser_GalleryImages_LikedImagesId",
                table: "GalleryImageUser",
                column: "LikedImagesId",
                principalTable: "GalleryImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GalleryImages_Users_UserCreatedId",
                table: "GalleryImages");

            migrationBuilder.DropForeignKey(
                name: "FK_GalleryImageUser_GalleryImages_LikedImagesId",
                table: "GalleryImageUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GalleryImages",
                table: "GalleryImages");

            migrationBuilder.RenameTable(
                name: "GalleryImages",
                newName: "UsersGallery");

            migrationBuilder.RenameIndex(
                name: "IX_GalleryImages_UserCreatedId",
                table: "UsersGallery",
                newName: "IX_UsersGallery_UserCreatedId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersGallery",
                table: "UsersGallery",
                column: "Id");

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
    }
}
