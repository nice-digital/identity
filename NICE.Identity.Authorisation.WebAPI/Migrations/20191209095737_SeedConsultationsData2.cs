using Microsoft.EntityFrameworkCore.Migrations;
using NICE.Identity.Authorisation.WebAPI.Configuration;

namespace NICE.Identity.Authorisation.WebAPI.Migrations
{
    public partial class SeedConsultationsData2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(@"
				DECLARE @CurrentEnvironmentName nvarchar(50) = '" + AppSettings.EnvironmentConfig.Name + @"'

				DELETE FROM UserRoles
				DELETE FROM Roles
				DELETE FROM Websites
				DELETE FROM [Services]

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

				DECLARE @WebsiteId_CommentsLocal int,
						@WebsiteId_CommentsDev int,
						@WebsiteId_CommentsTest int,
						@WebsiteId_CommentsAlpha int,
						@WebsiteId_CommentsBeta int,
						@WebsiteId_CommentsLive int;

				-- FIRST INSERT Service
				DECLARE @ServiceId_Comments int
				SELECT @ServiceId_Comments = ServiceId FROM [Services] WHERE [Name] = 'Consultation Comments'
				IF @ServiceId_Comments IS NULL
				BEGIN
					INSERT INTO [Services] ([Name])
					VALUES ('Consultation Comments')

					SET @ServiceId_Comments = SCOPE_IDENTITY()
				END

				--Now setup role data: 'Administrator', 'CommentAdminTeam', 'CHTETeam', 'CfGTeam', 'HSCTeam', 'IndevUser'
				--for the websites:  @WebsiteId_CommentsLocal, @WebsiteId_CommentsDev, @WebsiteId_CommentsTest, @WebsiteId_CommentsAlpha, @WebsiteId_CommentsBeta, @WebsiteId_CommentsLive
				DECLARE @RolesToInsert TABLE ([Name] nvarchar(100) NOT NULL, [Description] nvarchar(max) NOT NULL)
				INSERT INTO @RolesToInsert ([Name], [Description]) VALUES ('Administrator', 'The administrator is the super user of the website, with full access to everything')
				INSERT INTO @RolesToInsert ([Name], [Description]) VALUES ('CommentAdminTeam', 'The comment admin team user has access to the comments download page and question administration')
				INSERT INTO @RolesToInsert ([Name], [Description]) VALUES ('CHTETeam', 'The CHTE team user has access to the download page and can download comments and answers')
				INSERT INTO @RolesToInsert ([Name], [Description]) VALUES ('CfGTeam', 'The CfG team user has access to the download page and can download comments and answers')
				INSERT INTO @RolesToInsert ([Name], [Description]) VALUES ('HSCTeam', 'The HSC team user has access to the download page and can download comments and answers')
				INSERT INTO @RolesToInsert ([Name], [Description]) VALUES ('IndevUser', 'The IndevUser role has access to the download page and question administration') 


				-- NOW INSERT WEBSITES
				IF (@CurrentEnvironmentId = @environmentId_Local)
				BEGIN
					SELECT @WebsiteId_CommentsLocal = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_Comments AND EnvironmentID = @environmentId_Local 
					IF (@WebsiteId_CommentsLocal  IS NULL)
					BEGIN
						INSERT INTO Websites (ServiceID, EnvironmentID, Host)
						VALUES (@ServiceId_Comments, @environmentId_Local, 'niceorg')

						SET @WebsiteId_CommentsLocal = SCOPE_IDENTITY()
					END

					INSERT INTO Roles (WebsiteID, [Name], [Description])
					SELECT @WebsiteId_CommentsLocal, rti.[Name], rti.[Description]
					FROM @RolesToInsert rti
					WHERE NOT EXISTS 
					(
						SELECT 1
						FROM Roles r2
						WHERE r2.[Name] = rti.[Name]
						AND r2.WebsiteID = @WebsiteId_CommentsLocal
					)
				END

				IF (@CurrentEnvironmentId = @environmentId_Dev)
				BEGIN
					SELECT @WebsiteId_CommentsDev = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_Comments AND EnvironmentID = @environmentId_Dev 
					IF (@WebsiteId_CommentsDev  IS NULL)
					BEGIN
						INSERT INTO Websites (ServiceID, EnvironmentID, Host)
						VALUES (@ServiceId_Comments, @environmentId_Dev, 'dev.nice.org.uk')

						SET @WebsiteId_CommentsDev = SCOPE_IDENTITY()
					END

					INSERT INTO Roles (WebsiteID, [Name], [Description])
					SELECT @WebsiteId_CommentsDev, rti.[Name], rti.[Description]
					FROM @RolesToInsert rti
					WHERE NOT EXISTS 
					(
						SELECT 1
						FROM Roles r2
						WHERE r2.[Name] = rti.[Name]
						AND r2.WebsiteID = @WebsiteId_CommentsDev
					)
				END

				IF (@CurrentEnvironmentId = @environmentId_Test)
				BEGIN
					SELECT @WebsiteId_CommentsTest = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_Comments AND EnvironmentID = @environmentId_Test 
					IF (@WebsiteId_CommentsTest  IS NULL)
					BEGIN
						INSERT INTO Websites (ServiceID, EnvironmentID, Host)
						VALUES (@ServiceId_Comments, @environmentId_Test, 'test.nice.org.uk')

						SET @WebsiteId_CommentsTest = SCOPE_IDENTITY()
					END

					INSERT INTO Roles (WebsiteID, [Name], [Description])
					SELECT @WebsiteId_CommentsTest, rti.[Name], rti.[Description]
					FROM @RolesToInsert rti
					WHERE NOT EXISTS 
					(
						SELECT 1
						FROM Roles r2
						WHERE r2.[Name] = rti.[Name]
						AND r2.WebsiteID = @WebsiteId_CommentsTest
					)
				END

				IF (@CurrentEnvironmentId = @environmentId_Alpha)
				BEGIN
					SELECT @WebsiteId_CommentsAlpha = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_Comments AND EnvironmentID = @environmentId_Alpha 
					IF (@WebsiteId_CommentsAlpha  IS NULL)
					BEGIN
						INSERT INTO Websites (ServiceID, EnvironmentID, Host)
						VALUES (@ServiceId_Comments, @environmentId_Alpha, 'alpha.nice.org.uk')

						SET @WebsiteId_CommentsAlpha = SCOPE_IDENTITY()
					END

					INSERT INTO Roles (WebsiteID, [Name], [Description])
					SELECT @WebsiteId_CommentsAlpha, rti.[Name], rti.[Description]
					FROM @RolesToInsert rti
					WHERE NOT EXISTS 
					(
						SELECT 1
						FROM Roles r2
						WHERE r2.[Name] = rti.[Name]
						AND r2.WebsiteID = @WebsiteId_CommentsAlpha
					)
				END

				IF (@CurrentEnvironmentId = @environmentId_Beta)
				BEGIN
					SELECT @WebsiteId_CommentsBeta = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_Comments AND EnvironmentID = @environmentId_Beta 
					IF (@WebsiteId_CommentsBeta  IS NULL)
					BEGIN
						INSERT INTO Websites (ServiceID, EnvironmentID, Host)
						VALUES (@ServiceId_Comments, @environmentId_Beta, 'beta.nice.org.uk')

						SET @WebsiteId_CommentsBeta = SCOPE_IDENTITY()
					END

					INSERT INTO Roles (WebsiteID, [Name], [Description])
					SELECT @WebsiteId_CommentsBeta, rti.[Name], rti.[Description]
					FROM @RolesToInsert rti
					WHERE NOT EXISTS 
					(
						SELECT 1
						FROM Roles r2
						WHERE r2.[Name] = rti.[Name]
						AND r2.WebsiteID = @WebsiteId_CommentsBeta
					)
				END

				IF (@CurrentEnvironmentId = @environmentId_Live)
				BEGIN
					SELECT @WebsiteId_CommentsLive = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_Comments AND EnvironmentID = @environmentId_Live
					IF (@WebsiteId_CommentsLive  IS NULL)
					BEGIN
						INSERT INTO Websites (ServiceID, EnvironmentID, Host)
						VALUES (@ServiceId_Comments, @environmentId_Live, 'www.nice.org.uk')

						SET @WebsiteId_CommentsLive = SCOPE_IDENTITY()
					END

					INSERT INTO Roles (WebsiteID, [Name], [Description])
					SELECT @WebsiteId_CommentsLive, rti.[Name], rti.[Description]
					FROM @RolesToInsert rti
					WHERE NOT EXISTS 
					(
						SELECT 1
						FROM Roles r2
						WHERE r2.[Name] = rti.[Name]
						AND r2.WebsiteID = @WebsiteId_CommentsLive
					)
				END
			");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
