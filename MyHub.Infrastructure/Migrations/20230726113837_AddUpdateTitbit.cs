using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdateTitbit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "Titbits",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserUpdatedId",
                table: "Titbits",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Titbits_UserUpdatedId",
                table: "Titbits",
                column: "UserUpdatedId");

            migrationBuilder.AddForeignKey(
                name: "FK_Titbits_Users_UserUpdatedId",
                table: "Titbits",
                column: "UserUpdatedId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Titbits_Users_UserUpdatedId",
                table: "Titbits");

            migrationBuilder.DropIndex(
                name: "IX_Titbits_UserUpdatedId",
                table: "Titbits");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "Titbits");

            migrationBuilder.DropColumn(
                name: "UserUpdatedId",
                table: "Titbits");
        }
    }
}
