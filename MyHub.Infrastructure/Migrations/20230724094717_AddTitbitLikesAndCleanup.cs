using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTitbitLikesAndCleanup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GalleryImageUser_GalleryImages_LikedImagesId",
                table: "GalleryImageUser");

            migrationBuilder.DropForeignKey(
                name: "FK_GalleryImageUser_Users_LikedUsersId",
                table: "GalleryImageUser");

            migrationBuilder.RenameColumn(
                name: "LikedUsersId",
                table: "GalleryImageUser",
                newName: "LikedGalleryUsersId");

            migrationBuilder.RenameColumn(
                name: "LikedImagesId",
                table: "GalleryImageUser",
                newName: "LikedGalleryImagesId");

            migrationBuilder.RenameIndex(
                name: "IX_GalleryImageUser_LikedUsersId",
                table: "GalleryImageUser",
                newName: "IX_GalleryImageUser_LikedGalleryUsersId");

            migrationBuilder.CreateTable(
                name: "TitbitUser",
                columns: table => new
                {
                    LikedTitbitUsersId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LikedTitbitsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TitbitUser", x => new { x.LikedTitbitUsersId, x.LikedTitbitsId });
                    table.ForeignKey(
                        name: "FK_TitbitUser_Titbits_LikedTitbitsId",
                        column: x => x.LikedTitbitsId,
                        principalTable: "Titbits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TitbitUser_Users_LikedTitbitUsersId",
                        column: x => x.LikedTitbitUsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TitbitUser_LikedTitbitsId",
                table: "TitbitUser",
                column: "LikedTitbitsId");

            migrationBuilder.AddForeignKey(
                name: "FK_GalleryImageUser_GalleryImages_LikedGalleryImagesId",
                table: "GalleryImageUser",
                column: "LikedGalleryImagesId",
                principalTable: "GalleryImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GalleryImageUser_Users_LikedGalleryUsersId",
                table: "GalleryImageUser",
                column: "LikedGalleryUsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GalleryImageUser_GalleryImages_LikedGalleryImagesId",
                table: "GalleryImageUser");

            migrationBuilder.DropForeignKey(
                name: "FK_GalleryImageUser_Users_LikedGalleryUsersId",
                table: "GalleryImageUser");

            migrationBuilder.DropTable(
                name: "TitbitUser");

            migrationBuilder.RenameColumn(
                name: "LikedGalleryUsersId",
                table: "GalleryImageUser",
                newName: "LikedUsersId");

            migrationBuilder.RenameColumn(
                name: "LikedGalleryImagesId",
                table: "GalleryImageUser",
                newName: "LikedImagesId");

            migrationBuilder.RenameIndex(
                name: "IX_GalleryImageUser_LikedGalleryUsersId",
                table: "GalleryImageUser",
                newName: "IX_GalleryImageUser_LikedUsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_GalleryImageUser_GalleryImages_LikedImagesId",
                table: "GalleryImageUser",
                column: "LikedImagesId",
                principalTable: "GalleryImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GalleryImageUser_Users_LikedUsersId",
                table: "GalleryImageUser",
                column: "LikedUsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
