using Microsoft.EntityFrameworkCore.Migrations;

namespace NICE.Identity.Authorisation.WebAPI.Migrations
{
    public partial class RenamingAuth0UserIdToNameIdentifier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.RenameColumn("Auth0UserId", "Users", "NameIdentifier");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
