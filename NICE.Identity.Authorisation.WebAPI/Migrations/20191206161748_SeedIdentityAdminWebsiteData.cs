using Microsoft.EntityFrameworkCore.Migrations;

namespace NICE.Identity.Authorisation.WebAPI.Migrations
{
    public partial class SeedIdentityAdminWebsiteData : Migration
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

				DECLARE @ServiceId_IdentityAdmin int

				DECLARE @WebsiteId_IdentityAdminLocal int,
						@WebsiteId_IdentityAdminDev int,
						@WebsiteId_IdentityAdminTest int,
						@WebsiteId_IdentityAdminAlpha int,
						@WebsiteId_IdentityAdminBeta int,
						@WebsiteId_IdentityAdminLive int;

				-- FIRST INSERT Service

				SELECT @ServiceId_IdentityAdmin = ServiceId FROM [Services] WHERE [Name] = 'Identity Admin'
				IF @ServiceId_IdentityAdmin IS NULL
				BEGIN
					INSERT INTO [Services] ([Name])
					VALUES ('Identity Admin')

					SET @ServiceId_IdentityAdmin = SCOPE_IDENTITY()
				END

				-- NOW INSERT WEBSITES

				SELECT @WebsiteId_IdentityAdminLocal = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_IdentityAdmin AND EnvironmentID = @environmentId_Local 
				IF (@WebsiteId_IdentityAdminLocal  IS NULL)
				BEGIN
					INSERT INTO Websites (ServiceID, EnvironmentID, Host)
					VALUES (@ServiceId_IdentityAdmin, @environmentId_Local, 'local-identityadmin.nice.org.uk')

					SET @WebsiteId_IdentityAdminLocal = SCOPE_IDENTITY()
				END

				SELECT @WebsiteId_IdentityAdminDev = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_IdentityAdmin AND EnvironmentID = @environmentId_Dev 
				IF (@WebsiteId_IdentityAdminDev  IS NULL)
				BEGIN
					INSERT INTO Websites (ServiceID, EnvironmentID, Host)
					VALUES (@ServiceId_IdentityAdmin, @environmentId_Dev, 'dev-identityadmin.nice.org.uk')

					SET @WebsiteId_IdentityAdminDev = SCOPE_IDENTITY()
				END

				SELECT @WebsiteId_IdentityAdminTest = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_IdentityAdmin AND EnvironmentID = @environmentId_Test 
				IF (@WebsiteId_IdentityAdminTest  IS NULL)
				BEGIN
					INSERT INTO Websites (ServiceID, EnvironmentID, Host)
					VALUES (@ServiceId_IdentityAdmin, @environmentId_Test, 'test-identityadmin.nice.org.uk')

					SET @WebsiteId_IdentityAdminTest = SCOPE_IDENTITY()
				END

				SELECT @WebsiteId_IdentityAdminAlpha = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_IdentityAdmin AND EnvironmentID = @environmentId_Alpha 
				IF (@WebsiteId_IdentityAdminAlpha  IS NULL)
				BEGIN
					INSERT INTO Websites (ServiceID, EnvironmentID, Host)
					VALUES (@ServiceId_IdentityAdmin, @environmentId_Alpha, 'alpha-identityadmin.nice.org.uk')

					SET @WebsiteId_IdentityAdminAlpha = SCOPE_IDENTITY()
				END

				SELECT @WebsiteId_IdentityAdminBeta = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_IdentityAdmin AND EnvironmentID = @environmentId_Beta 
				IF (@WebsiteId_IdentityAdminBeta  IS NULL)
				BEGIN
					INSERT INTO Websites (ServiceID, EnvironmentID, Host)
					VALUES (@ServiceId_IdentityAdmin, @environmentId_Beta, 'beta-identityadmin.nice.org.uk')

					SET @WebsiteId_IdentityAdminBeta = SCOPE_IDENTITY()
				END

				SELECT @WebsiteId_IdentityAdminLive = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_IdentityAdmin AND EnvironmentID = @environmentId_Live
				IF (@WebsiteId_IdentityAdminLive  IS NULL)
				BEGIN
					INSERT INTO Websites (ServiceID, EnvironmentID, Host)
					VALUES (@ServiceId_IdentityAdmin, @environmentId_Live, 'identityadmin.nice.org.uk')

					SET @WebsiteId_IdentityAdminLive = SCOPE_IDENTITY()
				END

				--NOW INSERT Roles: 'User:Administration'
				--for the websites:  @WebsiteId_IdentityAdminLocal, @WebsiteId_IdentityAdminDev, @WebsiteId_IdentityAdminTest, @WebsiteId_IdentityAdminAlpha, @WebsiteId_IdentityAdminBeta, @WebsiteId_IdentityAdminLive

				DECLARE @RolesToInsert TABLE ([Name] nvarchar(100) NOT NULL, [Description] nvarchar(100))
				INSERT INTO @RolesToInsert ([Name], [Description]) VALUES ('Administrator', 'This user has full access and unrestricted access.')

				INSERT INTO Roles (WebsiteID, [Name], [Description])
				SELECT @WebsiteId_IdentityAdminLocal, rti.[Name], rti.[Description]
				FROM @RolesToInsert rti
				WHERE NOT EXISTS 
				(
					SELECT 1
					FROM Roles r2
					WHERE r2.[Name] = rti.[Name]
					AND r2.WebsiteID = @WebsiteId_IdentityAdminLocal
				)

				INSERT INTO Roles (WebsiteID, [Name], [Description])
				SELECT @WebsiteId_IdentityAdminDev, rti.[Name], rti.[Description]
				FROM @RolesToInsert rti
				WHERE NOT EXISTS 
				(
					SELECT 1
					FROM Roles r2
					WHERE r2.[Name] = rti.[Name]
					AND r2.WebsiteID = @WebsiteId_IdentityAdminDev
				)

				INSERT INTO Roles (WebsiteID, [Name], [Description])
				SELECT @WebsiteId_IdentityAdminTest, rti.[Name], rti.[Description]
				FROM @RolesToInsert rti
				WHERE NOT EXISTS 
				(
					SELECT 1
					FROM Roles r2
					WHERE r2.[Name] = rti.[Name]
					AND r2.WebsiteID = @WebsiteId_IdentityAdminTest
				)

				INSERT INTO Roles (WebsiteID, [Name], [Description])
				SELECT @WebsiteId_IdentityAdminAlpha, rti.[Name], rti.[Description]
				FROM @RolesToInsert rti
				WHERE NOT EXISTS 
				(
					SELECT 1
					FROM Roles r2
					WHERE r2.[Name] = rti.[Name]
					AND r2.WebsiteID = @WebsiteId_IdentityAdminAlpha
				)

				INSERT INTO Roles (WebsiteID, [Name], [Description])
				SELECT @WebsiteId_IdentityAdminBeta, rti.[Name], rti.[Description]
				FROM @RolesToInsert rti
				WHERE NOT EXISTS 
				(
					SELECT 1
					FROM Roles r2
					WHERE r2.[Name] = rti.[Name]
					AND r2.WebsiteID = @WebsiteId_IdentityAdminBeta
				)

				INSERT INTO Roles (WebsiteID, [Name], [Description])
				SELECT @WebsiteId_IdentityAdminLive, rti.[Name], rti.[Description]
				FROM @RolesToInsert rti
				WHERE NOT EXISTS 
				(
					SELECT 1
					FROM Roles r2
					WHERE r2.[Name] = rti.[Name]
					AND r2.WebsiteID = @WebsiteId_IdentityAdminLive
				)
			");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
