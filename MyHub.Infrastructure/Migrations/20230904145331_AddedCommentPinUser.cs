using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedCommentPinUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserPinnedId",
                table: "GalleryImageComments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GalleryImageComments_UserPinnedId",
                table: "GalleryImageComments",
                column: "UserPinnedId");

            migrationBuilder.AddForeignKey(
                name: "FK_GalleryImageComments_Users_UserPinnedId",
                table: "GalleryImageComments",
                column: "UserPinnedId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GalleryImageComments_Users_UserPinnedId",
                table: "GalleryImageComments");

            migrationBuilder.DropIndex(
                name: "IX_GalleryImageComments_UserPinnedId",
                table: "GalleryImageComments");

            migrationBuilder.DropColumn(
                name: "UserPinnedId",
                table: "GalleryImageComments");
        }
    }
}
