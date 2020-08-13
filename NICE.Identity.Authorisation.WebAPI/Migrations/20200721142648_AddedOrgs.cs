using Microsoft.EntityFrameworkCore.Migrations;

namespace NICE.Identity.Authorisation.WebAPI.Migrations
{
    public partial class AddedOrgs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Organisations",
                columns: table => new
                {
                    OrganisationID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisations", x => x.OrganisationID);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    JobID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(nullable: false),
                    OrganisationID = table.Column<int>(nullable: false),
                    JobTitle = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.JobID);
                    table.ForeignKey(
                        name: "FK_Jobs_Organisation",
                        column: x => x.OrganisationID,
                        principalTable: "Organisations",
                        principalColumn: "OrganisationID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Jobs_User",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationRoles",
                columns: table => new
                {
                    OrganisationRoleID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationID = table.Column<int>(nullable: false),
                    RoleID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationRoles", x => x.OrganisationRoleID);
                    table.ForeignKey(
                        name: "FK_OrganisationRole_Organisation",
                        column: x => x.OrganisationID,
                        principalTable: "Organisations",
                        principalColumn: "OrganisationID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganisationRole_Role",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_OrganisationID",
                table: "Jobs",
                column: "OrganisationID");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_UserID",
                table: "Jobs",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationRoles_OrganisationID",
                table: "OrganisationRoles",
                column: "OrganisationID");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationRoles_RoleID",
                table: "OrganisationRoles",
                column: "RoleID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "OrganisationRoles");

            migrationBuilder.DropTable(
                name: "Organisations");
        }
    }
}
