using Microsoft.EntityFrameworkCore.Migrations;

namespace NICE.Identity.Authorisation.WebAPI.Migrations
{
    public partial class EnvironmentOrderUpdateSQL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(@"UPDATE Environments
								SET [Order] = 1
								WHERE [Name] = 'Live'

								UPDATE Environments
								SET [Order] = 2
								WHERE [Name] = 'Beta'

								UPDATE Environments
								SET [Order] = 3
								WHERE [Name] = 'Alpha'

								UPDATE Environments
								SET [Order] = 4
								WHERE [Name] = 'Test'

								UPDATE Environments
								SET [Order] = 5
								WHERE [Name] = 'Dev'

								UPDATE Environments
								SET [Order] = 6
								WHERE [Name] = 'Local'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
