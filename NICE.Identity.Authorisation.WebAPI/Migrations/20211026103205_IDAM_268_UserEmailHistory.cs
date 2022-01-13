using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NICE.Identity.Authorisation.WebAPI.Migrations
{
    public partial class IDAM_268_UserEmailHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserEmailHistory",
                columns: table => new
                {
                    UserEmailHistoryID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(nullable: false),
                    EmailAddress = table.Column<string>(maxLength: 320, nullable: false),
                    ArchivedByUserID = table.Column<int>(nullable: true),
                    ArchivedDateUTC = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEmailHistory", x => x.UserEmailHistoryID);
                    table.ForeignKey(
                        name: "FK_UserEmailHistory_ArchivedByUser_Users",
                        column: x => x.ArchivedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserEmailHistory_User_Users",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserEmailHistory_ArchivedByUserID",
                table: "UserEmailHistory",
                column: "ArchivedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserEmailHistory_UserID",
                table: "UserEmailHistory",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserEmailHistory");
        }
    }
}
