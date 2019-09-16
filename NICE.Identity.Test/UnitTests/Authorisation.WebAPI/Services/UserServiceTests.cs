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
			var otherUserEmailAddress = "another@user.com";
			var newUserEmailAddress = "new@user.com";

			var context = GetContext();
			context.Users.Add(new User {EmailAddress = otherUserEmailAddress });
			context.SaveChanges();

			var userService = new UsersService(context, _logger.Object);
			var userToInsert = new CreateUser{ Email = newUserEmailAddress };

			//Act
			userService.CreateOrUpdateUser(userToInsert);

			//Assert
			var allUsers = context.Users.ToList();
			allUsers.Count.ShouldBe(2);
			allUsers.Count(user => user.EmailAddress == otherUserEmailAddress).ShouldBe(1);
			allUsers.Count( user => user.EmailAddress == newUserEmailAddress).ShouldBe(1);
		}

	    [Fact]
		public void Create_user_where_user_already_exists()
	    {
		    //Arrange
		    var newUserEmailAddress = "new@user.com";

		    var context = GetContext();
		    context.Users.Add(new User { EmailAddress = newUserEmailAddress, Auth0UserId = "not empty"});
		    context.SaveChanges();

		    var userService = new UsersService(context, _logger.Object);
		    var userToInsert = new CreateUser { Email = newUserEmailAddress };

		    //Act + Assert
		    Assert.Throws<Exception>(() => userService.CreateOrUpdateUser(userToInsert));
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
		    userService.CreateOrUpdateUser(userToInsert);

			//Assert
		    context.Users.Single().Auth0UserId.ShouldBe(auth0UserId);
	    }

        [Fact]
        public void Get_single_user_from_userId()
        {
            //Arrange
            const string newUserEmailAddress = "new@user.com";
            const string auth0UserId = "some value";
            var context = GetContext();
            var user = context.Users.Add(new User { EmailAddress = newUserEmailAddress, Auth0UserId = auth0UserId });
            context.SaveChanges();
            var userService = new UsersService(context, _logger.Object);

            //Act
            var actual = userService.GetUser(user.Entity.UserId);

            //Assert
            actual.EmailAddress.ShouldBe(newUserEmailAddress);
            actual.Auth0UserId.ShouldBe(auth0UserId);
        }
	}
}
