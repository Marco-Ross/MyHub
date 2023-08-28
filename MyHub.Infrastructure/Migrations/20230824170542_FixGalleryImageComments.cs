using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixGalleryImageComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GalleryImageComment_GalleryImages_ImageId",
                table: "GalleryImageComment");

            migrationBuilder.DropForeignKey(
                name: "FK_GalleryImageComment_Users_UserId",
                table: "GalleryImageComment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GalleryImageComment",
                table: "GalleryImageComment");

            migrationBuilder.RenameTable(
                name: "GalleryImageComment",
                newName: "GalleryImageComments");

            migrationBuilder.RenameIndex(
                name: "IX_GalleryImageComment_UserId",
                table: "GalleryImageComments",
                newName: "IX_GalleryImageComments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_GalleryImageComment_ImageId",
                table: "GalleryImageComments",
                newName: "IX_GalleryImageComments_ImageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GalleryImageComments",
                table: "GalleryImageComments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GalleryImageComments_GalleryImages_ImageId",
                table: "GalleryImageComments",
                column: "ImageId",
                principalTable: "GalleryImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GalleryImageComments_Users_UserId",
                table: "GalleryImageComments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GalleryImageComments_GalleryImages_ImageId",
                table: "GalleryImageComments");

            migrationBuilder.DropForeignKey(
                name: "FK_GalleryImageComments_Users_UserId",
                table: "GalleryImageComments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GalleryImageComments",
                table: "GalleryImageComments");

            migrationBuilder.RenameTable(
                name: "GalleryImageComments",
                newName: "GalleryImageComment");

            migrationBuilder.RenameIndex(
                name: "IX_GalleryImageComments_UserId",
                table: "GalleryImageComment",
                newName: "IX_GalleryImageComment_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_GalleryImageComments_ImageId",
                table: "GalleryImageComment",
                newName: "IX_GalleryImageComment_ImageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GalleryImageComment",
                table: "GalleryImageComment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GalleryImageComment_GalleryImages_ImageId",
                table: "GalleryImageComment",
                column: "ImageId",
                principalTable: "GalleryImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GalleryImageComment_Users_UserId",
                table: "GalleryImageComment",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
