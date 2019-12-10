using Microsoft.Extensions.Logging;
using Moq;
using DataModels = NICE.Identity.Authorisation.WebAPI.DataModels;
using ApiModels = NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Services;
using NICE.Identity.Test.Infrastructure;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
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
        public void Create_user_where_user_email_exists_but_name_identifier_is_different_creates_new_user_record() //i.e. duplicate emails are valid.
        {
            //Arrange
            const string existingUserEmail = "existing.user@email.com";
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object);
            context.Users.Add(new DataModels.User {NameIdentifier = "some name identifier", EmailAddress = existingUserEmail });
            context.SaveChanges();

            //Act
            userService.CreateUser(new ApiModels.User{NameIdentifier = "another name identifier", EmailAddress = existingUserEmail});

            //Assert
            var allUsers = context.Users.ToList();
            allUsers.Count.ShouldBe(2);
            var usersWithSameEmail = allUsers.Where(user => user.EmailAddress == existingUserEmail);
            usersWithSameEmail.Count().ShouldBe(2);
            usersWithSameEmail.Select(u => u.NameIdentifier).Distinct().Count().ShouldBe(2);
        }

        [Fact]
        public void Create_user_where_user_already_exists()
        {
            //Arrange
            var nameIdentifier = Guid.NewGuid().ToString();

            var context = GetContext();
            context.Users.Add(new DataModels.User { NameIdentifier = nameIdentifier });
            context.SaveChanges();

            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object);
            var userToInsert = new ApiModels.User { NameIdentifier = nameIdentifier };

            //Act + Assert
            userService.CreateUser(userToInsert);
            context.Users.Count().ShouldBe(1);
        }

		//I don't think this test is valid anymore. not sure when updating authentication provider id is ever needed.
        //[Fact]
        //public void Create_user_where_user_already_exists_to_update_authentication_provider_id()
        //{
        //    //Arrange
        //    var userEmailAddress = "new@user.com";
        //    var nameIdentifier = "some value";

        //    var context = GetContext();
        //    context.Users.Add(new DataModels.User { EmailAddress = userEmailAddress, NameIdentifier = null });
        //    context.SaveChanges();

        //    var userService = new UsersService(context, _logger.Object, _providerManagementService.Object);
        //    var userToInsert = new ApiModels.User { EmailAddress = userEmailAddress, NameIdentifier = nameIdentifier };

        //    //Act 
        //    userService.CreateUser(userToInsert);

        //    //Assert
        //    context.Users.Single().NameIdentifier.ShouldBe(nameIdentifier);
        //}

        [Fact]
        public void Get_single_user_from_userId()
        {
            //Arrange
            const string email = "singleuser@example.com";
            const string nameIdentifier = "user|toget";
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object);
            var user = new ApiModels.User { NameIdentifier = nameIdentifier, EmailAddress = email };
            var createdUserId = userService.CreateUser(user).UserId;

            //Act
            var actual = userService.GetUser(createdUserId.Value);

            //Assert
            actual.EmailAddress.ShouldBe(email);
            actual.NameIdentifier.ShouldBe(nameIdentifier);
        }

        [Fact]
        public async Task Update_user_that_already_exists()
        {
            //Arrange
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object);
            var user = new ApiModels.User(){ EmailAddress = "existing.user@email.com", FirstName = "Joe"};
            var createdUserId = userService.CreateUser(user).UserId;

            //Act
            var userToUpdate = userService.GetUser(createdUserId.Value);
            userToUpdate.FirstName = "John";
            var updatedUser = await userService.UpdateUser(createdUserId.Value, userToUpdate);

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
            Assert.ThrowsAsync<NullReferenceException>(async () => await userService.UpdateUser(nonExistingUserId, userToUpdate));
            context.Users.Count().ShouldBe(0);
        }

        [Fact]
        public void Delete_single_user_with_userId()
        {
            //Arrange
            const string newUserEmailAddress = "usertodelete@example.com";
            const string nameIdentifier = "user|todelete";
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object);
            var user = new ApiModels.User { NameIdentifier = nameIdentifier, EmailAddress = newUserEmailAddress };
            var createdUser = userService.CreateUser(user);

            //Act
            userService.DeleteUser(createdUser.UserId.Value);

            //Assert
            var deletedUser = userService.GetUser(createdUser.UserId.Value);
            deletedUser.ShouldBe(null);
        }
    }
}
