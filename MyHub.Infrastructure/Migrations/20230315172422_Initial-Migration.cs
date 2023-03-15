using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccessingUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordSalt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisterToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisterTokenSalt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResetPasswordToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResetPasswordTokenSalt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisterTokenExpireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResetPasswordTokenExpireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailVerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastResetPasswordDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsEmailVerified = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessingUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessingUsers_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Emails",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    To = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    From = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TemplateName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Emails_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountRegisterEmails",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountRegisterEmails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountRegisterEmails_Emails_Id",
                        column: x => x.Id,
                        principalTable: "Emails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PasswordRecoveryEmails",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordRecoveryEmails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordRecoveryEmails_Emails_Id",
                        column: x => x.Id,
                        principalTable: "Emails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Emails_UserId",
                table: "Emails",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessingUsers");

            migrationBuilder.DropTable(
                name: "AccountRegisterEmails");

            migrationBuilder.DropTable(
                name: "PasswordRecoveryEmails");

            migrationBuilder.DropTable(
                name: "Emails");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
