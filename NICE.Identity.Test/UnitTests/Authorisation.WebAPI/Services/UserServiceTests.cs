﻿using Microsoft.Extensions.Logging;
using Moq;
using DataModels = NICE.Identity.Authorisation.WebAPI.DataModels;
using ApiModels = NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Services;
using NICE.Identity.Test.Infrastructure;
using Shouldly;
using System;
using System.Collections.Generic;
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
            context.Users.Add(new DataModels.User { EmailAddress = newUserEmailAddress, NameIdentifier = "not empty"});
            context.SaveChanges();

            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object);
            var userToInsert = new ApiModels.User { EmailAddress = newUserEmailAddress };

            //Act + Assert
            userService.CreateUser(userToInsert);
            context.Users.Count().ShouldBe(1);
        }

        [Fact]
        public void Create_user_where_user_already_exists_to_update_authentication_provider_id()
        {
            //Arrange
            var userEmailAddress = "new@user.com";
            var nameIdentifier = "some value";

            var context = GetContext();
            context.Users.Add(new DataModels.User { EmailAddress = userEmailAddress, NameIdentifier = null });
            context.SaveChanges();

            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object);
            var userToInsert = new ApiModels.User { EmailAddress = userEmailAddress, NameIdentifier = nameIdentifier };

            //Act 
            userService.CreateUser(userToInsert);

            //Assert
            context.Users.Single().NameIdentifier.ShouldBe(nameIdentifier);
        }

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
        public async Task Delete_single_user_with_userId()
        {
            //Arrange
            const string newUserEmailAddress = "usertodelete@example.com";
            const string nameIdentifier = "user|todelete";
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object);
            var user = new ApiModels.User { NameIdentifier = nameIdentifier, EmailAddress = newUserEmailAddress };
            var createdUser = userService.CreateUser(user);

            //Act
            var deleteReturn = await userService.DeleteUser(createdUser.UserId.Value);

            //Assert
            var deletedUser = userService.GetUser(createdUser.UserId.Value);
            deletedUser.ShouldBe(null);
            deleteReturn.ShouldBe(1);
        }
        
        [Fact]
        public void Get_roles_for_user_by_website()
        {
            //Arrange
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object);
            TestData.AddEnvironment(ref context, 1);
            TestData.AddService(ref context, 1);
            TestData.AddWebsite(ref context, 1, 1, 1);
            TestData.AddRole(ref context, 1, 1, "TestRole");
            TestData.AddUser(ref context, 1);
            TestData.AddUserRole(ref context);
            context.SaveChanges();

            //Act
            var userRolesByWebsite =  userService.GetRolesForUserByWebsite(1, 1);

            //Assert
            userRolesByWebsite.Roles.Count().ShouldBe(1);
            userRolesByWebsite.Roles.First().Name.ShouldBe("TestRole");
            userRolesByWebsite.UserId.ShouldBe(1);
            userRolesByWebsite.WebsiteId.ShouldBe(1);
        }

        [Fact]
        public void Update_roles_for_user_by_website()
        {
            //Arrange
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object);
            TestData.AddEnvironment(ref context, 1);
            TestData.AddService(ref context, 1);
            TestData.AddWebsite(ref context, 1, 1, 1);
            TestData.AddRole(ref context, 1, 1, "TestRole1");
            TestData.AddRole(ref context, 2, 1, "TestRole2");
            TestData.AddRole(ref context, 3, 1, "TestRole3");
            TestData.AddUser(ref context, 1);
            context.UserRoles.Add(new DataModels.UserRole() {UserId = 1, RoleId = 1});
            context.UserRoles.Add(new DataModels.UserRole() {UserId = 1, RoleId = 2});
            context.SaveChanges();

            //Act
            userService.UpdateRolesForUserByWebsite(
                1,1, new ApiModels.UserRolesByWebsite()
                {
                    UserId = 1,
                    WebsiteId = 1,
                    Roles = new List<ApiModels.UserRoleDetailed>()
                    {
                        new ApiModels.UserRoleDetailed()
                        {
                            Id = 1,
                            HasRole = true
                        },
                        new ApiModels.UserRoleDetailed()
                        {
                            Id = 2,
                            HasRole = false
                        },
                        new ApiModels.UserRoleDetailed()
                        {
                            Id = 3,
                            HasRole = true
                        }
                    }
                });

            //Assert
            var userRolesByWebsite =  userService.GetRolesForUserByWebsite(1, 1);
            userRolesByWebsite.UserId.ShouldBe(1);
            userRolesByWebsite.WebsiteId.ShouldBe(1);
            userRolesByWebsite.Roles.Find(r => r.Id == 1).HasRole.ShouldBe(true);
            userRolesByWebsite.Roles.Find(r => r.Id == 2).HasRole.ShouldBe(false);
            userRolesByWebsite.Roles.Find(r => r.Id == 3).HasRole.ShouldBe(true);
        }
    }
}
