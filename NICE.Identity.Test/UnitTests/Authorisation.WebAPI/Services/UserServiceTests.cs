using Microsoft.Extensions.Logging;
using Moq;
using NICE.Identity.Authorisation.WebAPI.ApiModels.Requests;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using NICE.Identity.Authorisation.WebAPI.Services;
using NICE.Identity.Test.Infrastructure;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace NICE.Identity.Test.UnitTests.Authorisation.WebAPI.Services
{
    public class UserServiceTests : TestBase
    {
	    private readonly Mock<ILogger<UsersService>> _logger;

	    public UserServiceTests()
	    {
		    _logger = new Mock<ILogger<UsersService>>();
		}

	    [Fact]
		public void Create_user_where_user_does_not_already_exist()
		{
			//Arrange
			const string existingUserEmail = "existing.user@email.com";
			const string newUserEmail = "new.user@email.com";
			var context = GetContext();
            var userService = new UsersService(context, _logger.Object);
			context.Users.Add(new User {EmailAddress = existingUserEmail });
			context.SaveChanges();

            //Act
            var userToInsert = new CreateUser{ Email = newUserEmail };
			userService.CreateUser(userToInsert);

			//Assert
			var allUsers = context.Users.ToList();
			allUsers.Count.ShouldBe(2);
			allUsers.Count(user => user.EmailAddress == existingUserEmail).ShouldBe(1);
			allUsers.Count( user => user.EmailAddress == newUserEmail).ShouldBe(1);
		}

	    [Fact]
		public void Create_user_where_user_already_exists()
	    {
		    //Arrange
		    var newUserEmailAddress = "new.user.@email.com";

		    var context = GetContext();
		    context.Users.Add(new User { EmailAddress = newUserEmailAddress, Auth0UserId = "not empty"});
		    context.SaveChanges();

		    var userService = new UsersService(context, _logger.Object);
		    var userToInsert = new CreateUser { Email = newUserEmailAddress };

		    //Act + Assert
		    Assert.Throws<Exception>(() => userService.CreateUser(userToInsert));
		    context.Users.Count().ShouldBe(1);
	    }

	    [Fact]
	    public void Create_user_where_user_already_exists_to_update_authenticationprovider_id()
	    {
		    //Arrange
		    var newUserEmailAddress = "new@user.com";
		    var auth0UserId = "some value";

			var context = GetContext();
		    context.Users.Add(new User { EmailAddress = newUserEmailAddress, Auth0UserId = null });
		    context.SaveChanges();

		    var userService = new UsersService(context, _logger.Object);
		    var userToInsert = new CreateUser { Email = newUserEmailAddress, Auth0UserId = auth0UserId };

			//Act 
		    userService.CreateUser(userToInsert);

			//Assert
		    context.Users.Single().Auth0UserId.ShouldBe(auth0UserId);
	    }

        [Fact]
        public void Get_single_user_from_userId()
        {
            //Arrange
            const string email = "singleuser@example.com";
            const string auth0UserId = "user|toget";
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object);
            var user = new CreateUser { Auth0UserId = auth0UserId, Email = email };
            var createdUserId = userService.CreateUser(user).UserId;

            //Act
            var actual = userService.GetUser(createdUserId);

            //Assert
            actual.Email.ShouldBe(email);
            actual.Auth0UserId.ShouldBe(auth0UserId);
        }

        [Fact]
        public void Delete_single_user_with_userId()
        {
            //Arrange
            const string newUserEmailAddress = "usertodelete@example.com";
            const string auth0UserId = "user|todelete";
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object);
            var user = new CreateUser { Auth0UserId = auth0UserId, Email = newUserEmailAddress };
            var createdUser = userService.CreateUser(user);

            //Act
            userService.DeleteUser(createdUser.UserId);

            //Assert
            var deletedUser = userService.GetUser(createdUser.UserId);
            deletedUser.ShouldBe(null);
        }
        
        [Fact]
        public void Update_user_that_already_exists()
        {
            //Arrange
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object);
            var user = new CreateUser(){ Email = "existing.user@email.com", FirstName = "Joe"};
            var createdUser = userService.CreateUser(user);

            //Act
            var userToUpdate = context.Users.Find(createdUser.UserId);
            userToUpdate.FirstName = "John";
            var updatedUser = userService.UpdateUser(userToUpdate);

            //Assert
            var users = context.Users.ToList();
            users.Count.ShouldBe(1);
            
            updatedUser.FirstName.ShouldBe(userToUpdate.FirstName);
        }
    }
}
