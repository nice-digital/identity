using Microsoft.EntityFrameworkCore.Migrations;

namespace NICE.Identity.Authorisation.WebAPI.Migrations
{
    public partial class SeedIdentityAPIWebsiteData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(@"
				DECLARE	@environmentId_Local int,
						@environmentId_Dev int,
						@environmentId_Test int,
						@environmentId_Alpha int,
						@environmentId_Beta int,
						@environmentId_Live int;

				SELECT @environmentId_Local = EnvironmentId FROM Environments WHERE [Name] = 'Local'
				SELECT @environmentId_Dev = EnvironmentId FROM Environments WHERE [Name] = 'Dev'
				SELECT @environmentId_Test = EnvironmentId FROM Environments WHERE [Name] = 'Test'
				SELECT @environmentId_Alpha = EnvironmentId FROM Environments WHERE [Name] = 'Alpha'
				SELECT @environmentId_Beta = EnvironmentId FROM Environments WHERE [Name] = 'Beta'
				SELECT @environmentId_Live = EnvironmentId FROM Environments WHERE [Name] = 'Live'

				DECLARE @ServiceId_IdentityAPI int

				DECLARE @WebsiteId_IdentityAPILocal int,
						@WebsiteId_IdentityAPIDev int,
						@WebsiteId_IdentityAPITest int,
						@WebsiteId_IdentityAPIAlpha int,
						@WebsiteId_IdentityAPIBeta int,
						@WebsiteId_IdentityAPILive int;

				-- FIRST INSERT Service

				SELECT @ServiceId_IdentityAPI = ServiceId FROM [Services] WHERE [Name] = 'Identity API'
				IF @ServiceId_IdentityAPI IS NULL
				BEGIN
					INSERT INTO [Services] ([Name])
					VALUES ('Identity API')

					SET @ServiceId_IdentityAPI = SCOPE_IDENTITY()
				END

				-- NOW INSERT WEBSITES

				SELECT @WebsiteId_IdentityAPILocal = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_IdentityAPI AND EnvironmentID = @environmentId_Local 
				IF (@WebsiteId_IdentityAPILocal  IS NULL)
				BEGIN
					INSERT INTO Websites (ServiceID, EnvironmentID, Host)
					VALUES (@ServiceId_IdentityAPI, @environmentId_Local, 'localhost')

					SET @WebsiteId_IdentityAPILocal = SCOPE_IDENTITY()
				END

				SELECT @WebsiteId_IdentityAPIDev = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_IdentityAPI AND EnvironmentID = @environmentId_Dev 
				IF (@WebsiteId_IdentityAPIDev  IS NULL)
				BEGIN
					INSERT INTO Websites (ServiceID, EnvironmentID, Host)
					VALUES (@ServiceId_IdentityAPI, @environmentId_Dev, 'dev-identityapi.nice.org.uk')

					SET @WebsiteId_IdentityAPIDev = SCOPE_IDENTITY()
				END

				SELECT @WebsiteId_IdentityAPITest = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_IdentityAPI AND EnvironmentID = @environmentId_Test 
				IF (@WebsiteId_IdentityAPITest  IS NULL)
				BEGIN
					INSERT INTO Websites (ServiceID, EnvironmentID, Host)
					VALUES (@ServiceId_IdentityAPI, @environmentId_Test, 'test-identityapi.nice.org.uk')

					SET @WebsiteId_IdentityAPITest = SCOPE_IDENTITY()
				END

				SELECT @WebsiteId_IdentityAPIAlpha = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_IdentityAPI AND EnvironmentID = @environmentId_Alpha 
				IF (@WebsiteId_IdentityAPIAlpha  IS NULL)
				BEGIN
					INSERT INTO Websites (ServiceID, EnvironmentID, Host)
					VALUES (@ServiceId_IdentityAPI, @environmentId_Alpha, 'alpha-identityapi.nice.org.uk')

					SET @WebsiteId_IdentityAPIAlpha = SCOPE_IDENTITY()
				END

				SELECT @WebsiteId_IdentityAPIBeta = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_IdentityAPI AND EnvironmentID = @environmentId_Beta 
				IF (@WebsiteId_IdentityAPIBeta  IS NULL)
				BEGIN
					INSERT INTO Websites (ServiceID, EnvironmentID, Host)
					VALUES (@ServiceId_IdentityAPI, @environmentId_Beta, 'beta-identityapi.nice.org.uk')

					SET @WebsiteId_IdentityAPIBeta = SCOPE_IDENTITY()
				END

				SELECT @WebsiteId_IdentityAPILive = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_IdentityAPI AND EnvironmentID = @environmentId_Live
				IF (@WebsiteId_IdentityAPILive  IS NULL)
				BEGIN
					INSERT INTO Websites (ServiceID, EnvironmentID, Host)
					VALUES (@ServiceId_IdentityAPI, @environmentId_Live, 'identityapi.nice.org.uk')

					SET @WebsiteId_IdentityAPILive = SCOPE_IDENTITY()
				END

				--NOW INSERT Roles: 'User:Administration'
				--for the websites:  @WebsiteId_IdentityAPILocal, @WebsiteId_IdentityAPIDev, @WebsiteId_IdentityAPITest, @WebsiteId_IdentityAPIAlpha, @WebsiteId_IdentityAPIBeta, @WebsiteId_IdentityAPILive

				DECLARE @RolesToInsert TABLE ([Name] nvarchar(100) NOT NULL, [Description] nvarchar(100))
				INSERT INTO @RolesToInsert ([Name], [Description]) VALUES ('User:Administration', 'This role is required in order to access other users data in idam')

				INSERT INTO Roles (WebsiteID, [Name], [Description])
				SELECT @WebsiteId_IdentityAPILocal, rti.[Name], rti.[Description]
				FROM @RolesToInsert rti
				WHERE NOT EXISTS 
				(
					SELECT 1
					FROM Roles r2
					WHERE r2.[Name] = rti.[Name]
					AND r2.WebsiteID = @WebsiteId_IdentityAPILocal
				)

				INSERT INTO Roles (WebsiteID, [Name], [Description])
				SELECT @WebsiteId_IdentityAPIDev, rti.[Name], rti.[Description]
				FROM @RolesToInsert rti
				WHERE NOT EXISTS 
				(
					SELECT 1
					FROM Roles r2
					WHERE r2.[Name] = rti.[Name]
					AND r2.WebsiteID = @WebsiteId_IdentityAPIDev
				)

				INSERT INTO Roles (WebsiteID, [Name], [Description])
				SELECT @WebsiteId_IdentityAPITest, rti.[Name], rti.[Description]
				FROM @RolesToInsert rti
				WHERE NOT EXISTS 
				(
					SELECT 1
					FROM Roles r2
					WHERE r2.[Name] = rti.[Name]
					AND r2.WebsiteID = @WebsiteId_IdentityAPITest
				)

				INSERT INTO Roles (WebsiteID, [Name], [Description])
				SELECT @WebsiteId_IdentityAPIAlpha, rti.[Name], rti.[Description]
				FROM @RolesToInsert rti
				WHERE NOT EXISTS 
				(
					SELECT 1
					FROM Roles r2
					WHERE r2.[Name] = rti.[Name]
					AND r2.WebsiteID = @WebsiteId_IdentityAPIAlpha
				)

				INSERT INTO Roles (WebsiteID, [Name], [Description])
				SELECT @WebsiteId_IdentityAPIBeta, rti.[Name], rti.[Description]
				FROM @RolesToInsert rti
				WHERE NOT EXISTS 
				(
					SELECT 1
					FROM Roles r2
					WHERE r2.[Name] = rti.[Name]
					AND r2.WebsiteID = @WebsiteId_IdentityAPIBeta
				)

				INSERT INTO Roles (WebsiteID, [Name], [Description])
				SELECT @WebsiteId_IdentityAPILive, rti.[Name], rti.[Description]
				FROM @RolesToInsert rti
				WHERE NOT EXISTS 
				(
					SELECT 1
					FROM Roles r2
					WHERE r2.[Name] = rti.[Name]
					AND r2.WebsiteID = @WebsiteId_IdentityAPILive
				)
			");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}