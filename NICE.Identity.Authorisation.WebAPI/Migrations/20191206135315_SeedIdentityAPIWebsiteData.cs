using Microsoft.EntityFrameworkCore.Migrations;
using NICE.Identity.Authorisation.WebAPI.Configuration;

namespace NICE.Identity.Authorisation.WebAPI.Migrations
{
    public partial class SeedIdentityAPIWebsiteData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(@"
				DECLARE @CurrentEnvironmentName nvarchar(50) = '" + AppSettings.EnvironmentConfig.Name + @"'

				DECLARE @CurrentEnvironmentId int;
				SELECT @CurrentEnvironmentId = EnvironmentId 
				FROM Environments
				WHERE [Name] = @CurrentEnvironmentName

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

				DECLARE @WebsiteId_IdentityApiLocal int,
						@WebsiteId_IdentityApiDev int,
						@WebsiteId_IdentityApiTest int,
						@WebsiteId_IdentityApiAlpha int,
						@WebsiteId_IdentityApiBeta int,
						@WebsiteId_IdentityApiLive int;

				-- FIRST INSERT Service
				DECLARE @ServiceId_IdentityApi int
				SELECT @ServiceId_IdentityApi = ServiceId FROM [Services] WHERE [Name] = 'Identity API'
				IF @ServiceId_IdentityApi IS NULL
				BEGIN
					INSERT INTO [Services] ([Name])
					VALUES ('Identity API')

					SET @ServiceId_IdentityApi = SCOPE_IDENTITY()
				END

				--NOW INSERT Roles: 'User:Administration'
				--for the websites:  @WebsiteId_IdentityApiLocal, @WebsiteId_IdentityApiDev, @WebsiteId_IdentityApiTest, @WebsiteId_IdentityApiAlpha, @WebsiteId_IdentityApiBeta, @WebsiteId_IdentityApiLive
				DECLARE @RolesToInsert TABLE ([Name] nvarchar(100) NOT NULL, [Description] nvarchar(100))
				INSERT INTO @RolesToInsert ([Name], [Description]) VALUES ('User:Administration', 'This role is required in order to access other users data in idam')


				-- NOW INSERT WEBSITES
				IF (@CurrentEnvironmentId = @environmentId_Local)
				BEGIN
					SELECT @WebsiteId_IdentityApiLocal = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_IdentityApi AND EnvironmentID = @environmentId_Local 
					IF (@WebsiteId_IdentityApiLocal  IS NULL)
					BEGIN
						INSERT INTO Websites (ServiceID, EnvironmentID, Host)
						VALUES (@ServiceId_IdentityApi, @environmentId_Local, 'localhost')

						SET @WebsiteId_IdentityApiLocal = SCOPE_IDENTITY()
					END

					INSERT INTO Roles (WebsiteID, [Name], [Description])
					SELECT @WebsiteId_IdentityApiLocal, rti.[Name], rti.[Description]
					FROM @RolesToInsert rti
					WHERE NOT EXISTS 
					(
						SELECT 1
						FROM Roles r2
						WHERE r2.[Name] = rti.[Name]
						AND r2.WebsiteID = @WebsiteId_IdentityApiLocal
					)
				END

				IF (@CurrentEnvironmentId = @environmentId_Dev)
				BEGIN
					SELECT @WebsiteId_IdentityApiDev = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_IdentityApi AND EnvironmentID = @environmentId_Dev 
					IF (@WebsiteId_IdentityApiDev  IS NULL)
					BEGIN
						INSERT INTO Websites (ServiceID, EnvironmentID, Host)
						VALUES (@ServiceId_IdentityApi, @environmentId_Dev, 'dev-identityapi.nice.org.uk')

						SET @WebsiteId_IdentityApiDev = SCOPE_IDENTITY()
					END

					INSERT INTO Roles (WebsiteID, [Name], [Description])
					SELECT @WebsiteId_IdentityApiDev, rti.[Name], rti.[Description]
					FROM @RolesToInsert rti
					WHERE NOT EXISTS 
					(
						SELECT 1
						FROM Roles r2
						WHERE r2.[Name] = rti.[Name]
						AND r2.WebsiteID = @WebsiteId_IdentityApiDev
					)
				END

				IF (@CurrentEnvironmentId = @environmentId_Test)
				BEGIN
					SELECT @WebsiteId_IdentityApiTest = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_IdentityApi AND EnvironmentID = @environmentId_Test 
					IF (@WebsiteId_IdentityApiTest  IS NULL)
					BEGIN
						INSERT INTO Websites (ServiceID, EnvironmentID, Host)
						VALUES (@ServiceId_IdentityApi, @environmentId_Test, 'test-identityapi.nice.org.uk')

						SET @WebsiteId_IdentityApiTest = SCOPE_IDENTITY()
					END

					INSERT INTO Roles (WebsiteID, [Name], [Description])
					SELECT @WebsiteId_IdentityApiTest, rti.[Name], rti.[Description]
					FROM @RolesToInsert rti
					WHERE NOT EXISTS 
					(
						SELECT 1
						FROM Roles r2
						WHERE r2.[Name] = rti.[Name]
						AND r2.WebsiteID = @WebsiteId_IdentityApiTest
					)
				END

				IF (@CurrentEnvironmentId = @environmentId_Alpha)
				BEGIN
					SELECT @WebsiteId_IdentityApiAlpha = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_IdentityApi AND EnvironmentID = @environmentId_Alpha 
					IF (@WebsiteId_IdentityApiAlpha  IS NULL)
					BEGIN
						INSERT INTO Websites (ServiceID, EnvironmentID, Host)
						VALUES (@ServiceId_IdentityApi, @environmentId_Alpha, 'alpha-identityapi.nice.org.uk')

						SET @WebsiteId_IdentityApiAlpha = SCOPE_IDENTITY()
					END
						
					INSERT INTO Roles (WebsiteID, [Name], [Description])
					SELECT @WebsiteId_IdentityApiAlpha, rti.[Name], rti.[Description]
					FROM @RolesToInsert rti
					WHERE NOT EXISTS 
					(
						SELECT 1
						FROM Roles r2
						WHERE r2.[Name] = rti.[Name]
						AND r2.WebsiteID = @WebsiteId_IdentityApiAlpha
					)
				END

				IF (@CurrentEnvironmentId = @environmentId_Beta)
				BEGIN
					SELECT @WebsiteId_IdentityApiBeta = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_IdentityApi AND EnvironmentID = @environmentId_Beta 
					IF (@WebsiteId_IdentityApiBeta  IS NULL)
					BEGIN
						INSERT INTO Websites (ServiceID, EnvironmentID, Host)
						VALUES (@ServiceId_IdentityApi, @environmentId_Beta, 'beta-identityapi.nice.org.uk')

						SET @WebsiteId_IdentityApiBeta = SCOPE_IDENTITY()
					END

					INSERT INTO Roles (WebsiteID, [Name], [Description])
					SELECT @WebsiteId_IdentityApiBeta, rti.[Name], rti.[Description]
					FROM @RolesToInsert rti
					WHERE NOT EXISTS 
					(
						SELECT 1
						FROM Roles r2
						WHERE r2.[Name] = rti.[Name]
						AND r2.WebsiteID = @WebsiteId_IdentityApiBeta
					)
				END

				IF (@CurrentEnvironmentId = @environmentId_Live)
				BEGIN
					SELECT @WebsiteId_IdentityApiLive = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_IdentityApi AND EnvironmentID = @environmentId_Live
					IF (@WebsiteId_IdentityApiLive  IS NULL)
					BEGIN
						INSERT INTO Websites (ServiceID, EnvironmentID, Host)
						VALUES (@ServiceId_IdentityApi, @environmentId_Live, 'identityapi.nice.org.uk')

						SET @WebsiteId_IdentityApiLive = SCOPE_IDENTITY()
					END

					INSERT INTO Roles (WebsiteID, [Name], [Description])
					SELECT @WebsiteId_IdentityApiLive, rti.[Name], rti.[Description]
					FROM @RolesToInsert rti
					WHERE NOT EXISTS 
					(
						SELECT 1
						FROM Roles r2
						WHERE r2.[Name] = rti.[Name]
						AND r2.WebsiteID = @WebsiteId_IdentityApiLive
					)
				END
			");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}