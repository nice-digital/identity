using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NICE.Identity.Authorisation.WebAPI.Migrations
{
    public partial class IDAM167TidyUserIDAM170AcceptedTermsHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserRoles_UserID",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "DSActiveDirectoryUsername",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NICEAccountsID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NICEActiveDirectoryUsername",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "AcceptedTerms",
                table: "Users",
                newName: "IsMigrated");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Environments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "IX_UserRoles_UserID_RoleID",
                table: "UserRoles",
                columns: new[] { "UserID", "RoleID" });

            migrationBuilder.CreateTable(
                name: "TermsVersions",
                columns: table => new
                {
                    TermsVersionID = table.Column<int>(nullable: false),
                    VersionDate = table.Column<DateTime>(nullable: false),
                    CreatedByUserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermsVersions", x => x.TermsVersionID);
                    table.ForeignKey(
                        name: "FK_TermsVersion_CreatedByUser",
                        column: x => x.CreatedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAcceptedTermsVersion",
                columns: table => new
                {
                    UserAcceptedTermsVersionID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    TermsVersionID = table.Column<int>(nullable: false),
                    UserAcceptedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAcceptedTermsVersion", x => x.UserAcceptedTermsVersionID);
                    table.ForeignKey(
                        name: "FK_UserAcceptedTermsVersion_TermsVersion",
                        column: x => x.TermsVersionID,
                        principalTable: "TermsVersions",
                        principalColumn: "TermsVersionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAcceptedTermsVersion_User",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Environments",
                columns: new[] { "EnvironmentID", "Name", "Order" },
                values: new object[,]
                {
                    { 1, "Local", 0 },
                    { 2, "Dev", 0 },
                    { 3, "Test", 0 },
                    { 4, "Alpha", 0 },
                    { 5, "Beta", 0 },
                    { 6, "Live", 0 }
                });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "ServiceID", "Name" },
                values: new object[,]
                {
                    { 1, "NICE Website" },
                    { 2, "EPPI Reviewer v5" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TermsVersions_CreatedByUserID",
                table: "TermsVersions",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserAcceptedTermsVersion_TermsVersionID",
                table: "UserAcceptedTermsVersion",
                column: "TermsVersionID");

            migrationBuilder.CreateIndex(
                name: "IX_UserAcceptedTermsVersion_UserId",
                table: "UserAcceptedTermsVersion",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAcceptedTermsVersion");

            migrationBuilder.DropTable(
                name: "TermsVersions");

            migrationBuilder.DropUniqueConstraint(
                name: "IX_UserRoles_UserID_RoleID",
                table: "UserRoles");

            migrationBuilder.DeleteData(
                table: "Environments",
                keyColumn: "EnvironmentID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Environments",
                keyColumn: "EnvironmentID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Environments",
                keyColumn: "EnvironmentID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Environments",
                keyColumn: "EnvironmentID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Environments",
                keyColumn: "EnvironmentID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Environments",
                keyColumn: "EnvironmentID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "ServiceID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "ServiceID",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Environments");

            migrationBuilder.RenameColumn(
                name: "IsMigrated",
                table: "Users",
                newName: "AcceptedTerms");

            migrationBuilder.AddColumn<string>(
                name: "DSActiveDirectoryUsername",
                table: "Users",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NICEAccountsID",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NICEActiveDirectoryUsername",
                table: "Users",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Users",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserID",
                table: "UserRoles",
                column: "UserID");
        }
    }
}
