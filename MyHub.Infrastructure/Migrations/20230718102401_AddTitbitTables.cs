using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTitbitTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TitbitCategories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TitbitCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Titbits",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserCreatedId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CategoryId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateUploaded = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Titbits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Titbits_TitbitCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "TitbitCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Titbits_Users_UserCreatedId",
                        column: x => x.UserCreatedId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TitbitLinks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TitbitId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TitbitLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TitbitLinks_Titbits_TitbitId",
                        column: x => x.TitbitId,
                        principalTable: "Titbits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TitbitLinks_TitbitId",
                table: "TitbitLinks",
                column: "TitbitId");

            migrationBuilder.CreateIndex(
                name: "IX_Titbits_CategoryId",
                table: "Titbits",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Titbits_UserCreatedId",
                table: "Titbits",
                column: "UserCreatedId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TitbitLinks");

            migrationBuilder.DropTable(
                name: "Titbits");

            migrationBuilder.DropTable(
                name: "TitbitCategories");
        }
    }
}
