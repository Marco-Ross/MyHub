using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTempNewEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TemporaryNewEmail",
                table: "AccessingUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemporaryNewEmail",
                table: "AccessingUsers");
        }
    }
}
