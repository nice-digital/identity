using Microsoft.EntityFrameworkCore.Migrations;

namespace NICE.Identity.Authorisation.WebAPI.Migrations
{
    public partial class SeedConsultationsData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

			//The following script inserts a "Consultation Comments" service (if it doesn't exist)
			//then it inserts a website for that service for each of the environments - local, dev, test, alpha, beta and live. (if they don't exist)
			//then it inserts the roles 'Administrator', 'CommentAdminTeam', 'CHTETeam', 'CfGTeam', 'HSCTeam' for each of those websites (if they don't exist)
			migrationBuilder.Sql($@"
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
					INSERT INTO [Services] ([Name])
					VALUES ('Consultation Comments')

					SET @ServiceId_Comments = SCOPE_IDENTITY()
				END

				-- NOW INSERT WEBSITES

				SELECT @WebsiteId_CommentsLocal = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_Comments AND EnvironmentID = @environmentId_Local 
				IF (@WebsiteId_CommentsLocal  IS NULL)
				BEGIN
					INSERT INTO Websites (ServiceID, EnvironmentID, Host)
					VALUES (@ServiceId_Comments, @environmentId_Local, 'niceorg')

					SET @WebsiteId_CommentsLocal = SCOPE_IDENTITY()
				END

				SELECT @WebsiteId_CommentsDev = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_Comments AND EnvironmentID = @environmentId_Dev 
				IF (@WebsiteId_CommentsDev  IS NULL)
				BEGIN
					INSERT INTO Websites (ServiceID, EnvironmentID, Host)
					VALUES (@ServiceId_Comments, @environmentId_Dev, 'dev.nice.org.uk')

					SET @WebsiteId_CommentsDev = SCOPE_IDENTITY()
				END

				SELECT @WebsiteId_CommentsTest = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_Comments AND EnvironmentID = @environmentId_Test 
				IF (@WebsiteId_CommentsTest  IS NULL)
				BEGIN
					INSERT INTO Websites (ServiceID, EnvironmentID, Host)
					VALUES (@ServiceId_Comments, @environmentId_Test, 'test.nice.org.uk')

					SET @WebsiteId_CommentsTest = SCOPE_IDENTITY()
				END

				SELECT @WebsiteId_CommentsAlpha = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_Comments AND EnvironmentID = @environmentId_Alpha 
				IF (@WebsiteId_CommentsAlpha  IS NULL)
				BEGIN
					INSERT INTO Websites (ServiceID, EnvironmentID, Host)
					VALUES (@ServiceId_Comments, @environmentId_Alpha, 'alpha.nice.org.uk')

					SET @WebsiteId_CommentsAlpha = SCOPE_IDENTITY()
				END

				SELECT @WebsiteId_CommentsBeta = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_Comments AND EnvironmentID = @environmentId_Beta 
				IF (@WebsiteId_CommentsBeta  IS NULL)
				BEGIN
					INSERT INTO Websites (ServiceID, EnvironmentID, Host)
					VALUES (@ServiceId_Comments, @environmentId_Beta, 'beta.nice.org.uk')

					SET @WebsiteId_CommentsBeta = SCOPE_IDENTITY()
				END

				SELECT @WebsiteId_CommentsLive = WebsiteId FROM Websites WHERE ServiceID = @ServiceId_Comments AND EnvironmentID = @environmentId_Live
				IF (@WebsiteId_CommentsLive  IS NULL)
				BEGIN
					INSERT INTO Websites (ServiceID, EnvironmentID, Host)
					VALUES (@ServiceId_Comments, @environmentId_Live, 'www.nice.org.uk')

					SET @WebsiteId_CommentsLive = SCOPE_IDENTITY()
				END

				--NOW INSERT Roles: 'Administrator', 'CommentAdminTeam', 'CHTETeam', 'CfGTeam', 'HSCTeam'
				--for the websites:  @WebsiteId_CommentsLocal, @WebsiteId_CommentsDev, @WebsiteId_CommentsTest, @WebsiteId_CommentsAlpha, @WebsiteId_CommentsBeta, @WebsiteId_CommentsLive

				DECLARE @RolesToInsert TABLE ([Name] nvarchar(100) NOT NULL )
				INSERT INTO @RolesToInsert ([Name]) VALUES ('Administrator')
				INSERT INTO @RolesToInsert ([Name]) VALUES ('CommentAdminTeam')
				INSERT INTO @RolesToInsert ([Name]) VALUES ('CHTETeam')
				INSERT INTO @RolesToInsert ([Name]) VALUES ('CfGTeam')
				INSERT INTO @RolesToInsert ([Name]) VALUES ('HSCTeam')

				INSERT INTO Roles (WebsiteID, [Name])
				SELECT @WebsiteId_CommentsLocal, rti.[Name]
				FROM @RolesToInsert rti
				WHERE NOT EXISTS 
				(
					SELECT 1
					FROM Roles r2
					WHERE r2.[Name] = rti.[Name]
					AND r2.WebsiteID = @WebsiteId_CommentsLocal
				)

				INSERT INTO Roles (WebsiteID, [Name])
				SELECT @WebsiteId_CommentsDev, rti.[Name]
				FROM @RolesToInsert rti
				WHERE NOT EXISTS 
				(
					SELECT 1
					FROM Roles r2
					WHERE r2.[Name] = rti.[Name]
					AND r2.WebsiteID = @WebsiteId_CommentsDev
				)

				INSERT INTO Roles (WebsiteID, [Name])
				SELECT @WebsiteId_CommentsTest, rti.[Name]
				FROM @RolesToInsert rti
				WHERE NOT EXISTS 
				(
					SELECT 1
					FROM Roles r2
					WHERE r2.[Name] = rti.[Name]
					AND r2.WebsiteID = @WebsiteId_CommentsTest
				)

				INSERT INTO Roles (WebsiteID, [Name])
				SELECT @WebsiteId_CommentsAlpha, rti.[Name]
				FROM @RolesToInsert rti
				WHERE NOT EXISTS 
				(
					SELECT 1
					FROM Roles r2
					WHERE r2.[Name] = rti.[Name]
					AND r2.WebsiteID = @WebsiteId_CommentsAlpha
				)

				INSERT INTO Roles (WebsiteID, [Name])
				SELECT @WebsiteId_CommentsBeta, rti.[Name]
				FROM @RolesToInsert rti
				WHERE NOT EXISTS 
				(
					SELECT 1
					FROM Roles r2
					WHERE r2.[Name] = rti.[Name]
					AND r2.WebsiteID = @WebsiteId_CommentsBeta
				)

				INSERT INTO Roles (WebsiteID, [Name])
				SELECT @WebsiteId_CommentsLive, rti.[Name]
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
