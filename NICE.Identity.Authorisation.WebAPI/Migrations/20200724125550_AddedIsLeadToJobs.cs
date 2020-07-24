using Microsoft.EntityFrameworkCore.Migrations;

namespace NICE.Identity.Authorisation.WebAPI.Migrations
{
    public partial class AddedIsLeadToJobs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "Jobs");

            migrationBuilder.AddColumn<bool>(
                name: "IsLead",
                table: "Jobs",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLead",
                table: "Jobs");

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "Jobs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
