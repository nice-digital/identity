using Microsoft.Extensions.Logging;
using Moq;
using DataModels = NICE.Identity.Authorisation.WebAPI.DataModels;
using ApiModels = NICE.Identity.Authorisation.WebAPI.ApiModels;
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
        private readonly Mock<IProviderManagementService> _providerManagementService;

        public UserServiceTests()
        {
            _logger = new Mock<ILogger<UsersService>>();
            _providerManagementService = new Mock<IProviderManagementService>();
        }

        [Fact]
        public void Create_user_where_user_does_not_already_exist()
        {
            //Arrange
            const string existingUserEmail = "existing.user@email.com";
            const string newUserEmail = "new.user@email.com";
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object);
            context.Users.Add(new DataModels.User {EmailAddress = existingUserEmail });
            context.SaveChanges();

            //Act
            userService.CreateUser(new ApiModels.User{EmailAddress = newUserEmail});

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
            context.Users.Add(new DataModels.User { EmailAddress = newUserEmailAddress, Auth0UserId = "not empty"});
            context.SaveChanges();

            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object);
            var userToInsert = new ApiModels.User { EmailAddress = newUserEmailAddress };

            //Act + Assert
            Assert.Throws<Exception>(() => userService.CreateUser(userToInsert));
            context.Users.Count().ShouldBe(1);
        }

        [Fact]
        public void Create_user_where_user_already_exists_to_update_authentication_provider_id()
        {
            //Arrange
            var userEmailAddress = "new@user.com";
            var auth0UserId = "some value";

            var context = GetContext();
            context.Users.Add(new DataModels.User { EmailAddress = userEmailAddress, Auth0UserId = null });
            context.SaveChanges();

            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object);
            var userToInsert = new ApiModels.User { EmailAddress = userEmailAddress, Auth0UserId = auth0UserId };

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
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object);
            var user = new ApiModels.User { Auth0UserId = auth0UserId, EmailAddress = email };
            var createdUserId = userService.CreateUser(user).UserId;

            //Act
            var actual = userService.GetUser(createdUserId);

            //Assert
            actual.EmailAddress.ShouldBe(email);
            actual.Auth0UserId.ShouldBe(auth0UserId);
        }

        [Fact]
        public void Update_user_that_already_exists()
        {
            //Arrange
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object);
            var user = new ApiModels.User(){ EmailAddress = "existing.user@email.com", FirstName = "Joe"};
            var createdUserId = userService.CreateUser(user).UserId;

            //Act
            var userToUpdate = userService.GetUser(createdUserId);
            userToUpdate.FirstName = "John";
            var updatedUser = userService.UpdateUser(createdUserId, userToUpdate);

            //Assert
            context.Users.ToList().Count.ShouldBe(1);
            updatedUser.FirstName.ShouldBe("John");
        }

        [Fact]
        public void Update_user_that_does_not_exist()
        {
            //Arrange
            const int nonExistingUserId = 987;
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object);
            var userToUpdate = new ApiModels.User()
            {
                UserId = nonExistingUserId, 
                EmailAddress = "random.user@email.com", 
                FirstName = "Joe"
            };

            //Act + Assert
            Assert.Throws<NullReferenceException>(() => userService.UpdateUser(nonExistingUserId, userToUpdate));
            context.Users.Count().ShouldBe(0);
        }

        [Fact]
        public void Delete_single_user_with_userId()
        {
            //Arrange
            const string newUserEmailAddress = "usertodelete@example.com";
            const string auth0UserId = "user|todelete";
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object);
            var user = new ApiModels.User { Auth0UserId = auth0UserId, EmailAddress = newUserEmailAddress };
            var createdUser = userService.CreateUser(user);

            //Act
            userService.DeleteUser(createdUser.UserId);

            //Assert
            var deletedUser = userService.GetUser(createdUser.UserId);
            deletedUser.ShouldBe(null);
        }
    }
}
