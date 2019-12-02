using Microsoft.EntityFrameworkCore.Migrations;

namespace NICE.Identity.Authorisation.WebAPI.Migrations
{
    public partial class AddingRoleDescriptionsForCommentsRoles : Migration
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

				DECLARE @ServiceId_Comments int

				DECLARE @WebsiteId_CommentsLocal int,
						@WebsiteId_CommentsDev int,
						@WebsiteId_CommentsTest int,
						@WebsiteId_CommentsAlpha int,
						@WebsiteId_CommentsBeta int,
						@WebsiteId_CommentsLive int;

				-- FIRST INSERT Service

				SELECT @ServiceId_Comments = ServiceId FROM [Services] WHERE [Name] = 'Consultation Comments'
				IF @ServiceId_Comments IS NULL
				BEGIN
					RAISERROR('service not found', 18, 1);
				END

				-- NOW INSERT WEBSITES

				SELECT @WebsiteId_CommentsLocal = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_Comments AND EnvironmentID = @environmentId_Local 
				IF (@WebsiteId_CommentsLocal  IS NULL)
				BEGIN
					RAISERROR('local website not found', 18, 1);
				END

				SELECT @WebsiteId_CommentsDev = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_Comments AND EnvironmentID = @environmentId_Dev 
				IF (@WebsiteId_CommentsDev  IS NULL)
				BEGIN
					RAISERROR('dev website not found', 18, 1);
				END

				SELECT @WebsiteId_CommentsTest = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_Comments AND EnvironmentID = @environmentId_Test 
				IF (@WebsiteId_CommentsTest  IS NULL)
				BEGIN
					RAISERROR('test website not found', 18, 1);
				END

				SELECT @WebsiteId_CommentsAlpha = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_Comments AND EnvironmentID = @environmentId_Alpha 
				IF (@WebsiteId_CommentsAlpha  IS NULL)
				BEGIN
					RAISERROR('alpha website not found', 18, 1);
				END

				SELECT @WebsiteId_CommentsBeta = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_Comments AND EnvironmentID = @environmentId_Beta 
				IF (@WebsiteId_CommentsBeta  IS NULL)
				BEGIN
					RAISERROR('beta website not found', 18, 1);
				END

				SELECT @WebsiteId_CommentsLive = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_Comments AND EnvironmentID = @environmentId_Live
				IF (@WebsiteId_CommentsLive  IS NULL)
				BEGIN
					RAISERROR('live website not found', 18, 1);
				END

				DECLARE @WebsiteIds TABLE ([WebsiteID] int NOT NULL)

				INSERT INTO @WebsiteIds ([WebsiteID])
				SELECT WebsiteID
				FROM Websites
				WHERE	WebsiteID = @WebsiteId_CommentsLocal OR 
						WebsiteID = @WebsiteId_CommentsDev OR
						WebsiteID = @WebsiteId_CommentsTest OR
						WebsiteID = @WebsiteId_CommentsAlpha OR
						WebsiteID = @WebsiteId_CommentsBeta OR
						WebsiteID = @WebsiteId_CommentsLive


				--NOW UPDATE Role descriptions
				--for the websites:  @WebsiteId_CommentsLocal, @WebsiteId_CommentsDev, @WebsiteId_CommentsTest, @WebsiteId_CommentsAlpha, @WebsiteId_CommentsBeta, @WebsiteId_CommentsLive

				UPDATE Roles 
				SET [Description] = 'The administrator is the super user of the website, with full access to everything'
				WHERE [Name] = 'Administrator' AND WebsiteID IN (SELECT WebsiteID FROM @WebsiteIds)

				UPDATE Roles 
				SET [Description] = 'The comment admin team user has access to the comments download page and question administration'
				WHERE [Name] = 'CommentAdminTeam' AND WebsiteID IN (SELECT WebsiteID FROM @WebsiteIds)

				UPDATE Roles 
				SET [Description] = 'The CHTE team user has access to the download page and can download comments and answers'
				WHERE [Name] = 'CHTETeam' AND WebsiteID IN (SELECT WebsiteID FROM @WebsiteIds)

				UPDATE Roles 
				SET [Description] = 'The CfG team user has access to the download page and can download comments and answers'
				WHERE [Name] = 'CfGTeam' AND WebsiteID IN (SELECT WebsiteID FROM @WebsiteIds)

				UPDATE Roles 
				SET [Description] = 'The HSC team user has access to the download page and can download comments and answers'
				WHERE [Name] = 'HSCTeam' AND WebsiteID IN (SELECT WebsiteID FROM @WebsiteIds)


				--NOW INSERT Roles: 'IndevUser'
				--for the websites:  @WebsiteId_CommentsLocal, @WebsiteId_CommentsDev, @WebsiteId_CommentsTest, @WebsiteId_CommentsAlpha, @WebsiteId_CommentsBeta, @WebsiteId_CommentsLive

				DECLARE @RolesToInsert TABLE ([Name] nvarchar(100) NOT NULL, [Description] nvarchar(max) NOT NULL)
				INSERT INTO @RolesToInsert ([Name], [Description]) VALUES ('IndevUser', 'The IndevUser role has access to the download page and question administration') --new 


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
			");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}