using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddThirdPartyDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "AccessingUsers");

            migrationBuilder.DropColumn(
                name: "ThirdPartyIdToken",
                table: "AccessingUsers");

            migrationBuilder.DropColumn(
                name: "ThirdPartyIssuerId",
                table: "AccessingUsers");

            migrationBuilder.CreateTable(
                name: "ThirdPartyDetails",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ThirdPartyIssuerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThirdPartyIdToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThirdPartyAccessToken = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThirdPartyDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThirdPartyDetails_AccessingUsers_Id",
                        column: x => x.Id,
                        principalTable: "AccessingUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThirdPartyDetails");

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "AccessingUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ThirdPartyIdToken",
                table: "AccessingUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThirdPartyIssuerId",
                table: "AccessingUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
