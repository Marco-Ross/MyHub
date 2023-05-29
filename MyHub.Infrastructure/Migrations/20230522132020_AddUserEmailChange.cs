using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserEmailChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChangeEmailToken",
                table: "AccessingUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ChangeEmailTokenExpireDate",
                table: "AccessingUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChangeEmailTokenSalt",
                table: "AccessingUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangeEmailToken",
                table: "AccessingUsers");

            migrationBuilder.DropColumn(
                name: "ChangeEmailTokenExpireDate",
                table: "AccessingUsers");

            migrationBuilder.DropColumn(
                name: "ChangeEmailTokenSalt",
                table: "AccessingUsers");
        }
    }
}
