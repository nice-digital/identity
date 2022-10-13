using Microsoft.Extensions.Logging;
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
using NICE.Identity.Authorisation.WebAPI;
using NICE.Identity.Authorisation.WebAPI.Repositories;
using Xunit;
using Xunit.Sdk;

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
        public void Create_user_where_user_email_exists_but_name_identifier_is_different_and_existing_user_is_migrated_does_not_create_new_user() 
        {
            //Arrange
            const string existingUserEmail = "existing.user@email.com";
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);
            context.Users.Add(new DataModels.User {NameIdentifier = "some name identifier", EmailAddress = existingUserEmail, IsMigrated = true, IsInAuthenticationProvider = false});
            context.SaveChanges();

            //Act
            userService.CreateUser(new ApiModels.User{NameIdentifier = "another name identifier", EmailAddress = existingUserEmail});

            //Assert
            var allUsers = context.Users.ToList();
            allUsers.Count.ShouldBe(1);
        }

        [Fact]
        public void Create_user_where_user_email_exists_but_name_identifier_is_different_and_existing_user_is_not_migrated() 
        {
	        //Arrange
	        const string existingUserEmail = "existing.user@email.com";
	        var context = GetContext();
	        var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);
	        context.Users.Add(new DataModels.User { NameIdentifier = "some name identifier", EmailAddress = existingUserEmail, IsMigrated = false, IsInAuthenticationProvider = true });
	        context.SaveChanges();

	        //Act + Assert
            Assert.Throws<DuplicateEmailException>(() => userService.CreateUser(new ApiModels.User { NameIdentifier = "another name identifier", EmailAddress = existingUserEmail }));
        }

        [Fact]
        public void Create_user_where_user_already_exists()
        {
            //Arrange
            var nameIdentifier = Guid.NewGuid().ToString();

            var context = GetContext();
            context.Users.Add(new DataModels.User { NameIdentifier = nameIdentifier });
            context.SaveChanges();

            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);
            var userToInsert = new ApiModels.User { NameIdentifier = nameIdentifier };

            //Act + Assert
            userService.CreateUser(userToInsert);
            context.Users.Count().ShouldBe(1);
        }

        [Fact]
        public void Get_single_user_from_userId()
        {
            //Arrange
            const string email = "singleuser@example.com";
            const string nameIdentifier = "user|toget";
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);
            var user = new ApiModels.User { NameIdentifier = nameIdentifier, EmailAddress = email };
            var createdUserId = userService.CreateUser(user).UserId;

            //Act
            var actual = userService.GetUser(createdUserId.Value);

            //Assert
            actual.EmailAddress.ShouldBe(email);
            actual.NameIdentifier.ShouldBe(nameIdentifier);
        }

        [Fact]
        public void Get_users()
        {
            //Arrange
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);
            
            var user1 = new ApiModels.User { NameIdentifier = "auth|user1", EmailAddress = "user1@example.com" };
            var user2 = new ApiModels.User { NameIdentifier = "auth|user2", EmailAddress = "user2@example.com" };
            var createdUser1 = userService.CreateUser(user1);
            var createdUser2 = userService.CreateUser(user2);

            //Act
            var users = userService.GetUsers();

            //Assert
            context.Users.Count().ShouldBe(2);
            users.Count().ShouldBe(2);
            users.First(u => u.UserId == createdUser1.UserId).NameIdentifier.ShouldBe("auth|user1");
            users.First(u => u.UserId == createdUser2.UserId).NameIdentifier.ShouldBe("auth|user2");
        }
        
        [Fact]
        public void Get_users_with_filter()
        {
            //Arrange
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);
            
            userService.CreateUser(new ApiModels.User 
            {
                NameIdentifier = "auth|user2",
                FirstName = "FirstName2",
                LastName = "LastName2",
                EmailAddress = "user2@example.com"
            });
            userService.CreateUser(new ApiModels.User
            {
	            NameIdentifier = "auth|user1",
	            FirstName = "FirstName1",
	            LastName = "LastName1",
	            EmailAddress = "user1@example.com"
            });
            context.SaveChanges();

            //Act
            var usersFilterByFirstName = userService.GetUsers("FirstName1");
            var usersFilterByLastName = userService.GetUsers("LastName2");
            var usersFilterByEmailAddress = userService.GetUsers("user1@example.com");
            var usersFilterByNameIdentifier = userService.GetUsers("auth|user1");
            var usersFilterMultiple = userService.GetUsers("example.com");
            var usersFilterWithFirstNameAndLastName = userService.GetUsers("Firstname1 Lastname1");

            //Assert
            context.Users.Count().ShouldBe(2);
            
            //note: users are returned sorted by most recently added first

            usersFilterByFirstName.Count.ShouldBe(1);
            usersFilterByFirstName.First().NameIdentifier.ShouldBe("auth|user1");

            usersFilterByLastName.Count.ShouldBe(1);
            usersFilterByLastName.First().NameIdentifier.ShouldBe("auth|user2");

            usersFilterByEmailAddress.Count.ShouldBe(1);
            usersFilterByEmailAddress.First().NameIdentifier.ShouldBe("auth|user1");
            
            usersFilterByNameIdentifier.Count.ShouldBe(1);
            usersFilterByNameIdentifier.First().NameIdentifier.ShouldBe("auth|user1");

            usersFilterMultiple.Count.ShouldBe(2);
            usersFilterMultiple.First().NameIdentifier.ShouldBe("auth|user1");
            usersFilterMultiple.Last().NameIdentifier.ShouldBe("auth|user2");

            usersFilterWithFirstNameAndLastName.Count.ShouldBe(1);
            usersFilterWithFirstNameAndLastName.Single().NameIdentifier.ShouldBe("auth|user1");
        }

        [Fact]
        public void Get_users_with_filter_not_found()
        {
            //Arrange
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);
            
            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = "auth|user1",
                FirstName = "FirstName1",
                LastName = "LastName1",
                EmailAddress = "user1@example.com"
            });
            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = "auth|user2",
                FirstName = "FirstName2",
                LastName = "LastName2",
                EmailAddress = "user2@example.com"
            });
            context.SaveChanges();

            //Act
            var usersFilterNotFound = userService.GetUsers("Name3");

            //Assert
            context.Users.Count().ShouldBe(2);
            usersFilterNotFound.Count().ShouldBe(0);
            usersFilterNotFound.ShouldNotBe(null);
        }

        private void AddUsersWithAndWithoutRoles(IdentityContext context)
        {
	        context.Environments.Add(new DataModels.Environment() { EnvironmentId = 1, Name = "Test" });
	        context.Services.Add(new DataModels.Service() { ServiceId = 1, Name = "Service" });
	        context.Websites.Add(new DataModels.Website()
	        {
		        WebsiteId = 1,
		        EnvironmentId = 1,
		        ServiceId = 1,
		        Host = "test.nice.org.uk"
	        });
	        context.Roles.Add(new DataModels.Role() { RoleId = 1, WebsiteId = 1, Name = "TestRole" });
	        context.Users.Add(new DataModels.User() { UserId = 1, NameIdentifier = "user with role" });
	        context.UserRoles.Add(new DataModels.UserRole() { UserRoleId = 1, RoleId = 1, UserId = 1 });

	        context.Users.Add(new DataModels.User() { UserId = 2, NameIdentifier = "user without role" });

            context.SaveChanges();
        }

        [Fact]
        public void Find_users_returns_all_users_when_unfiltered()
        {
	        //Arrange
	        var context = GetContext();
	        AddUsersWithAndWithoutRoles(context);

            //Act
            var usersFound = context.FindUsers("");

            //Assert
            usersFound.Count().ShouldBe(2);
        }

        [Fact]
        public void Users_with_roles_populate_Service_Ids_in_model()
        {
	        //Arrange
	        var context = GetContext();
	        AddUsersWithAndWithoutRoles(context);
	        var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);

            //Act
            var usersFound = userService.GetUsers("user");
            
	        //Assert
	        usersFound.Count().ShouldBe(2);
	        usersFound.Single(u => u.UserId == 1).HasAccessToWebsiteIds.Single().ShouldBe(1);
	        usersFound.Single(u => u.UserId == 2).HasAccessToWebsiteIds.Count().ShouldBe(0);
        }

        [Fact]
        public void Get_users_by_organisation_id()
        {
            //Arrange
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);

            var user1 = new ApiModels.User { UserId = 1, NameIdentifier = "auth|user1", EmailAddress = "user1@example.com" };
            var createdUser1 = userService.CreateUser(user1);

            var user2 = new ApiModels.User { UserId = 2, NameIdentifier = "auth|user2", EmailAddress = "user2@example.com" };
            var createdUser2 = userService.CreateUser(user2);

            var Joblogger = new Mock<ILogger<JobsService>>();
            var jobsService = new JobsService(context, Joblogger.Object);
            TestData.AddOrganisation(ref context);

            var createdJob1 = jobsService.CreateJob(new ApiModels.Job()
            {
                UserId = 1,
                OrganisationId = 1,
                IsLead = true
            });

            var createdJob2 = jobsService.CreateJob(new ApiModels.Job()
            {
                UserId = 2,
                OrganisationId = 2,
                IsLead = true
            });

            //Act
            var users = userService.GetUsersByOrganisationId(1);

            //Assert
            context.Users.Count().ShouldBe(2);
            users.Count().ShouldBe(1);
            users.First(u => u.UserId == createdUser1.UserId).NameIdentifier.ShouldBe("auth|user1");
        }

        [Fact]
        public async Task Update_user_that_already_exists()
        {
            //Arrange
            var originalEmailAddress = "original.email.address@email.com";
            var changedEmailAddress = "changed.email.address@email.com";
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);
            var user = new ApiModels.User(){ EmailAddress = originalEmailAddress, FirstName = "Joe"};
            var createdUserId = userService.CreateUser(user).UserId;

            //Act
            var userToUpdate = userService.GetUser(createdUserId.Value);
            userToUpdate.FirstName = "John";
            userToUpdate.EmailAddress = changedEmailAddress;
            var updatedUser = await userService.UpdateUser(createdUserId.Value, userToUpdate, null);

            //Assert
            context.Users.ToList().Count.ShouldBe(1);
            context.UserEmailHistory.ToList().Count.ShouldBe(1);
            updatedUser.FirstName.ShouldBe("John");
            updatedUser.EmailAddress.ShouldBe(changedEmailAddress);
            var emailRecord = context.UserEmailHistory.Single();
            emailRecord.UserId.ShouldBe(updatedUser.UserId);
            emailRecord.EmailAddress.ShouldBe(originalEmailAddress);
        }

        [Fact]
        public void Update_user_that_does_not_exist()
        {
            //Arrange
            const int nonExistingUserId = 987;
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);
            var userToUpdate = new ApiModels.User()
            {
                UserId = nonExistingUserId, 
                EmailAddress = "random.user@email.com", 
                FirstName = "Joe"
            };

            //Act + Assert
            Assert.ThrowsAsync<NullReferenceException>(async () => await userService.UpdateUser(nonExistingUserId, userToUpdate, null));
            context.Users.Count().ShouldBe(0);
        }

        [Fact]
        public async Task Delete_single_user_with_userId()
        {
            //Arrange
            const string newUserEmailAddress = "usertodelete@example.com";
            const string nameIdentifier = "user|todelete";
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);
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
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);
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
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);
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

        [Fact]
        public void Get_roles_for_user()
        {
            //Arrange
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);
            TestData.AddEnvironment(ref context, 1);
            TestData.AddService(ref context, 1);
            TestData.AddWebsite(ref context, 1, 1, 1);
            TestData.AddRole(ref context, 1, 1, "TestRole1");
            TestData.AddRole(ref context, 2, 1, "TestRole2");
            TestData.AddUser(ref context, 1);
            TestData.AddUserRole(ref context, 1, 1, 1);
            TestData.AddUserRole(ref context, 2, 2, 1);
            context.SaveChanges();

            //Act
            var userRoles =  userService.GetRolesForUser(1);

            //Assert
            userRoles.Count().ShouldBe(2);
            userRoles.First(r=>r.UserRoleId == 1).RoleId.ShouldBe(1);
            userRoles.First(r=>r.UserRoleId == 2).RoleId.ShouldBe(2);
        }
        
        [Fact]
        public void Update_roles_for_user()
        {
            //Arrange
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);
            context.Environments.Add(new DataModels.Environment() { EnvironmentId = 1});
            context.Services.Add(new DataModels.Service(){ServiceId = 1});
            context.Websites.Add(new DataModels.Website(){WebsiteId = 1, ServiceId = 1, EnvironmentId = 1});
            context.Roles.Add(new DataModels.Role() { RoleId = 1, Name = "TestRole1"});
            context.Roles.Add(new DataModels.Role() { RoleId = 2, Name = "TestRole2"});
            context.Users.Add(new DataModels.User(){UserId = 1});
            context.SaveChanges();

            //Act
            var userRoles =  userService.UpdateRolesForUser(1, new List<ApiModels.UserRole>()
            {
                new ApiModels.UserRole(){UserId = 1, RoleId = 1},
                new ApiModels.UserRole(){UserId = 1, RoleId = 2}
            });

            //Assert
            userRoles.Count().ShouldBe(2);
            userRoles.SingleOrDefault(ur => ur.UserId == 1 && ur.RoleId == 1).ShouldNotBeNull();
            userRoles.SingleOrDefault(ur => ur.UserId == 1 && ur.RoleId == 2).ShouldNotBeNull();
        }

        [Fact]
        public void Find_users()
        {
            //Arrange
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);
            context.Environments.Add(new DataModels.Environment() { EnvironmentId = 1, Name = "Dev"});
            context.Environments.Add(new DataModels.Environment() { EnvironmentId = 2, Name = "Test"});
            context.Services.Add(new DataModels.Service(){ServiceId = 1, Name = "Service"});
            context.Websites.Add(new DataModels.Website()
            {
                WebsiteId = 1, EnvironmentId = 1, ServiceId = 1, Host = "dev.nice.org.uk"
            });
            context.Roles.Add(new DataModels.Role() {RoleId = 1, WebsiteId = 1, Name = "TestRole1"});
            context.Roles.Add(new DataModels.Role() {RoleId = 2, WebsiteId = 1, Name = "TestRole2"});
            context.Users.Add(new DataModels.User(){UserId = 1, NameIdentifier = "auth|user1"});
            context.Users.Add(new DataModels.User(){UserId = 2, NameIdentifier = "auth|user2"});
            context.SaveChanges();

            //Act
            var users = userService.FindUsers(new []{"auth|user1","auth|user2"});

            //Assert
            users.Count().ShouldBe(2);
            users[0].NameIdentifier.ShouldBe("auth|user1");
            users[1].NameIdentifier.ShouldBe("auth|user2");
        }

        [Fact]
        public void Find_roles()
        {
            //Arrange
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);
            context.Environments.Add(new DataModels.Environment() { EnvironmentId = 1, Name = "Test"});
            context.Services.Add(new DataModels.Service(){ServiceId = 1, Name = "Service"});
            context.Websites.Add(new DataModels.Website()
            {
                WebsiteId = 1, EnvironmentId = 1, ServiceId = 1, Host = "test.nice.org.uk"
            });
            context.Roles.Add(new DataModels.Role() {RoleId = 1, WebsiteId = 1, Name = "TestRole"});
            context.Users.Add(new DataModels.User(){UserId = 1, NameIdentifier = "auth|user1"});
            context.UserRoles.Add(new DataModels.UserRole() { UserRoleId = 1, RoleId = 1, UserId = 1});
            context.SaveChanges();

            //Act
            var users = userService.FindRoles(new []{"auth|user1","auth|user2"}, "test.nice.org.uk");

            //Assert
            users["auth|user1"].First().ShouldBe("TestRole");
        }
        
        [Fact]
        public void Import_users()
        {
            //Arrange
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);
            context.Environments.Add(new DataModels.Environment() { EnvironmentId = 1, Name = "Test"});
            context.Services.Add(new DataModels.Service(){ServiceId = 1, Name = "Service"});
            context.Websites.Add(new DataModels.Website()
            {
                WebsiteId = 1, EnvironmentId = 1, ServiceId = 1, Host = "test.nice.org.uk"
            });
            context.Roles.Add(new DataModels.Role() {RoleId = 1, WebsiteId = 1, Name = "TestRole1"});
            context.SaveChanges();

            //Act
            userService.ImportUsers(new List<DataModels.ImportUser>()
            {
                new DataModels.ImportUser()
                {
                    UserId = Guid.NewGuid().ToString(),
                    FirstName = "FirstName",
                    LastName = "LastName",
                    EmailAddress = "FirstName.LastName@nice.org.uk",
                    Roles = new List<DataModels.ImportRole>()
                    {
                        new DataModels.ImportRole(){RoleName = "TestRole1", WebsiteHost = "test.nice.org.uk"},
                    }
                },
            });

            //Assert
            context.Users.Count().ShouldBe(1);
            var user = context.Users.First();
            user.FirstName.ShouldBe("FirstName");
            context.UserRoles.First(ur => ur.User.UserId == user.UserId).Role.Name.ShouldBe("TestRole1");
        }

        [Fact]
        public void Import_users_when_existing_user_available_does_not_create_duplicate()
        {
	        //Arrange
	        var context = GetContext();
	        var existingEmailAddress = "existing.user.account@nice.org.uk";
	        var existingFirstName = "Phil";
	        var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);
	        context.Users.Add(new DataModels.User() { EmailAddress = existingEmailAddress, FirstName = existingFirstName, LastName = "Connors"});
            context.SaveChanges();

	        //Act
	        userService.ImportUsers(new List<DataModels.ImportUser>()
	        {
		        new DataModels.ImportUser()
		        {
			        UserId = Guid.NewGuid().ToString(),
			        FirstName = "Same",
			        LastName = "User",
			        EmailAddress = existingEmailAddress
		        },
	        });

	        //Assert
	        context.Users.Count().ShouldBe(1);
	        var user = context.Users.First();
	        user.FirstName.ShouldBe(existingFirstName);
        }

        [Fact]
        public void Import_users_when_existing_user_available_does_not_create_duplicate_and_sets_roles_correctly()
        {
	        //Arrange
	        var context = GetContext();
	        var existingEmailAddress = "existing.user.account@nice.org.uk";
	        var existingFirstName = "Phil";
	        var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);
	        context.Users.Add(new DataModels.User() { EmailAddress = existingEmailAddress, FirstName = existingFirstName, LastName = "Connors" });
	        context.Environments.Add(new DataModels.Environment() { EnvironmentId = 1, Name = "Test" });
	        context.Services.Add(new DataModels.Service() { ServiceId = 1, Name = "Service" });
	        context.Websites.Add(new DataModels.Website()
	        {
		        WebsiteId = 1,
		        EnvironmentId = 1,
		        ServiceId = 1,
		        Host = "test.nice.org.uk"
	        });
	        context.Roles.Add(new DataModels.Role() { RoleId = 1, WebsiteId = 1, Name = "TestRole1" });
            context.SaveChanges();

	        //Act
	        userService.ImportUsers(new List<DataModels.ImportUser>()
	        {
		        new DataModels.ImportUser()
		        {
			        UserId = Guid.NewGuid().ToString(),
			        FirstName = "Same",
			        LastName = "User",
			        EmailAddress = existingEmailAddress,
			        Roles = new List<DataModels.ImportRole>()
			        {
				        new DataModels.ImportRole(){RoleName = "TestRole1", WebsiteHost = "test.nice.org.uk"},
			        }
                },
	        });

	        //Assert
	        context.Users.Count().ShouldBe(1);
	        var user = context.Users.First();
	        user.FirstName.ShouldBe(existingFirstName);
	        context.UserRoles.First(ur => ur.User.UserId == user.UserId).Role.Name.ShouldBe("TestRole1");
        }

		[Fact]
        public async Task TestPendingRegistrationDeletion()
        {
	        //Arrange
	        var context = GetContext();
	        var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);
	        const string user1NameIdentifier = "auth|user1";
	        const string user2NameIdentifier = "auth|user2";

            userService.CreateUser(new ApiModels.User
	        {
		        NameIdentifier = user1NameIdentifier,
		        FirstName = "FirstName1",
		        LastName = "LastName1",
		        EmailAddress = "user1@example.com",
                HasVerifiedEmailAddress = false
	        });
	        userService.CreateUser(new ApiModels.User
	        {
		        NameIdentifier = user2NameIdentifier,
		        FirstName = "User to be deleted",
		        LastName = "",
		        EmailAddress = "user2@example.com",
		        HasVerifiedEmailAddress = false
	        });
            context.Users.Single(u => u.NameIdentifier == user2NameIdentifier).InitialRegistrationDate = DateTime.Now.AddDays(-31);
            context.SaveChanges();
            //now add some related data to test that related entities are deleted too.
            var userToBeDeleted = context.Users.Single(u => u.NameIdentifier == user2NameIdentifier);
            const int organisationId = 1;
            context.Organisations.Add(new DataModels.Organisation(organisationId, "org 1"));
            context.Jobs.Add(new DataModels.Job(){ IsLead = false, OrganisationId = organisationId, UserId = userToBeDeleted.UserId });
            context.TermsVersions.Add(new DataModels.TermsVersion() {TermsVersionId = 1, VersionDate = DateTime.Now});
            context.UserAcceptedTermsVersions.Add(new DataModels.UserAcceptedTermsVersion(){ TermsVersionId = 1, UserAcceptedDate = DateTime.Now, UserId = userToBeDeleted.UserId });
            context.SaveChanges();

            //Act
            await userService.DeleteRegistrationsOlderThan(notify: false, daysToKeepPendingRegistration: 30);

	        //Assert
	        context.Users.Count().ShouldBe(1);
	        context.Users.Single().NameIdentifier.ShouldBe(user1NameIdentifier);
        }

        [Fact]
        public async Task TestInactiveAccounts()
        {
            //Arrange
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);
            const string user1NameIdentifier = "auth|user1";
            const string user2NameIdentifier = "auth|user2";
            const string user3NameIdentifier = "auth|user3";

            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = user1NameIdentifier,
                FirstName = "FirstName1",
                LastName = "LastName1",
                EmailAddress = "user1@example.com",
                HasVerifiedEmailAddress = true,
                LastLoggedInDate = DateTime.UtcNow
            });


            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = user2NameIdentifier,
                FirstName = "User to be deleted",
                LastName = "",
                EmailAddress = "user2@example.com",
                HasVerifiedEmailAddress = true,
                LastLoggedInDate = DateTime.UtcNow.AddYears(-3)
            });

            userService.CreateUser(new ApiModels.User
            {
                NameIdentifier = user3NameIdentifier,
                FirstName = "User not to be deleted",
                LastName = "",
                EmailAddress = "user3@nice.org.uk",
                HasVerifiedEmailAddress = true,
                LastLoggedInDate = DateTime.UtcNow.AddYears(-3)
            });
            context.Users.Single(u => u.NameIdentifier == user2NameIdentifier).LastLoggedInDate = DateTime.UtcNow.AddYears(-3);
            context.SaveChanges();
            //now add some related data to test that related entities are deleted too.
            var userToBeDeleted = context.Users.Single(u => u.NameIdentifier == user2NameIdentifier);
            const int organisationId = 1;
            context.Organisations.Add(new DataModels.Organisation(organisationId, "org 1"));
            context.Jobs.Add(new DataModels.Job() { IsLead = false, OrganisationId = organisationId, UserId = userToBeDeleted.UserId });
            context.TermsVersions.Add(new DataModels.TermsVersion() { TermsVersionId = 1, VersionDate = DateTime.Now });
            context.UserAcceptedTermsVersions.Add(new DataModels.UserAcceptedTermsVersion() { TermsVersionId = 1, UserAcceptedDate = DateTime.Now, UserId = userToBeDeleted.UserId });
            context.SaveChanges();

            //Act
            await userService.DeleteInactiveAccountsOlderThan(notify: false, yearsToKeepInactiveAcounts: 3);

            //Assert
            context.Users.Count().ShouldBe(2);
            context.Users.Where(u => u.NameIdentifier == user1NameIdentifier).ShouldNotBeNull();
            context.Users.Where(u => u.NameIdentifier == user3NameIdentifier).ShouldNotBeNull();
            context.Jobs.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetUserWithSingleEmailAuditRecord()
        {
	        //Arrange
	        const string originalEmailAddress = "original@example.com";
	        const string updatedEmailAddress = "updated@example.com";
            const string adminNameIdentifier = "admin";
            const string userNameIdentifier = "user";
            var context = GetContext();
	        var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);

	        var adminUser = new ApiModels.User { NameIdentifier = adminNameIdentifier, EmailAddress = "admin@example.com"};
	        var createdAdminUser = userService.CreateUser(adminUser);

            var user = new ApiModels.User { NameIdentifier = userNameIdentifier, EmailAddress = originalEmailAddress };
	        var createdUser = userService.CreateUser(user);
	        createdUser.EmailAddress = updatedEmailAddress;
	        var updatedUser = await userService.UpdateUser(createdUser.UserId.Value, createdUser, adminNameIdentifier);

            //Act
            var userWithAudit = userService.GetUser(updatedUser.UserId.Value);

            //Assert
            userWithAudit.EmailAddress.ShouldBe(updatedEmailAddress);
            var emailHistoryRecord = userWithAudit.UserEmailHistory.Single();
            
            emailHistoryRecord.UserId.ShouldBe(createdUser.UserId);
            emailHistoryRecord.EmailAddress.ShouldBe(originalEmailAddress);

            emailHistoryRecord.ArchivedByUserId.ShouldBe(createdAdminUser.UserId);
            emailHistoryRecord.ArchivedByUser.NameIdentifier.ShouldBe(adminNameIdentifier);
        }

        [Fact]
        public async Task GetUserWithMultipleEmailAuditRecord()
        {
	        //Arrange
	        const string originalEmailAddress = "original@example.com";
	        const string updatedEmailAddress1 = "updated@example.com";
	        const string updatedEmailAddress2 = "second.update@example.com";
            const string adminNameIdentifier = "admin";
	        const string userNameIdentifier = "user";
	        var context = GetContext();
	        var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);

	        var adminUser = new ApiModels.User { NameIdentifier = adminNameIdentifier, EmailAddress = "admin@example.com" };
	        var createdAdminUser = userService.CreateUser(adminUser);

	        var user = new ApiModels.User { NameIdentifier = userNameIdentifier, EmailAddress = originalEmailAddress };
	        var createdUser = userService.CreateUser(user);
	        createdUser.EmailAddress = updatedEmailAddress1;
	        var updatedUser = await userService.UpdateUser(createdUser.UserId.Value, createdUser, adminNameIdentifier);
	        updatedUser.EmailAddress = updatedEmailAddress2;
	        updatedUser = await userService.UpdateUser(createdUser.UserId.Value, updatedUser, adminNameIdentifier);

            //Act
            var userWithAudit = userService.GetUser(updatedUser.UserId.Value);

	        //Assert
	        userWithAudit.EmailAddress.ShouldBe(updatedEmailAddress2);
	        userWithAudit.UserEmailHistory.Count().ShouldBe(2);
	        var firstEmailHistoryRecord = userWithAudit.UserEmailHistory.First();
	        var secondEmailHistoryRecord = userWithAudit.UserEmailHistory.Skip(1).First();

	        firstEmailHistoryRecord.UserId.ShouldBe(createdUser.UserId);
	        firstEmailHistoryRecord.EmailAddress.ShouldBe(originalEmailAddress);

	        firstEmailHistoryRecord.ArchivedByUserId.ShouldBe(createdAdminUser.UserId);
	        firstEmailHistoryRecord.ArchivedByUser.NameIdentifier.ShouldBe(adminNameIdentifier);

	        secondEmailHistoryRecord.UserId.ShouldBe(createdUser.UserId);
	        secondEmailHistoryRecord.EmailAddress.ShouldBe(updatedEmailAddress1);

	        secondEmailHistoryRecord.ArchivedByUserId.ShouldBe(createdAdminUser.UserId);
	        secondEmailHistoryRecord.ArchivedByUser.NameIdentifier.ShouldBe(adminNameIdentifier);
        }

        [Fact]
        public async Task Find_users_finds_users_based_on_old_emails()
        {
            //Arrange
            const string originalEmailAddress = "original@example.com";
            const string updatedEmailAddress = "updated@example.com";
            const string adminNameIdentifier = "admin";
            const string userNameIdentifier = "user";
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);
            
            var user = new ApiModels.User { NameIdentifier = userNameIdentifier, EmailAddress = originalEmailAddress };
            var createdUser = userService.CreateUser(user);
            createdUser.EmailAddress = updatedEmailAddress;
            await userService.UpdateUser(createdUser.UserId.Value, createdUser, adminNameIdentifier);

            //Act
            var usersFound = context.FindUsers(originalEmailAddress);

	        //Assert
	        usersFound.Single().EmailAddress.ShouldBe(updatedEmailAddress);
        }

        [Fact]
        public void Updating_email_address_to_an_existing_current_email_address_throws_exception()
        {
	        //Arrange
	        const string user1Email = "user1s.email@example.com";
	        var context = GetContext();
	        var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);

	        
	        var user1 = new ApiModels.User { NameIdentifier = "user1", EmailAddress = user1Email };
	        userService.CreateUser(user1);

	        var user2 = new ApiModels.User { NameIdentifier = "user2", EmailAddress = "user2s.email@example.com" };
	        var createdUser2 = userService.CreateUser(user2);

	        createdUser2.EmailAddress = user1Email;

            //Act + Assert
            Assert.ThrowsAsync<ApplicationException>(async() => await userService.UpdateUser(createdUser2.UserId.Value, createdUser2, null));
            context.Users.Count(u => u.EmailAddress.Equals(user1Email)).ShouldBe(1);
        }

        [Fact]
        public async Task Updating_email_address_to_an_previous_email_of_same_user_works()
        {
	        //Arrange
	        const string firstEmail = "first.email@example.com";
	        const string secondEmail = "second.email@example.com";

            var context = GetContext();
	        var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);


	        var userModel = new ApiModels.User { NameIdentifier = "user1", EmailAddress = firstEmail };
	        var createdUser = userService.CreateUser(userModel);

	        createdUser.EmailAddress = secondEmail;
	        var updatedUser = await userService.UpdateUser(createdUser.UserId.Value, createdUser, null);

	        updatedUser.EmailAddress = firstEmail;

            //Act
            var updatedAgainUser = await userService.UpdateUser(updatedUser.UserId.Value, updatedUser, null);

            //Assert
            updatedAgainUser.EmailAddress.ShouldBe(firstEmail);
        }

        [Fact]
        public void Get_users_and_jobs_for_an_organisation()
        {
            //Arrange
            var organisationId = 1;
            var context = GetContext();
            var userService = new UsersService(context, _logger.Object, _providerManagementService.Object, null);
            
            context.Organisations.Add(new DataModels.Organisation() { OrganisationId = organisationId, Name = "My Organisation" });
            context.Organisations.Add(new DataModels.Organisation() { OrganisationId = 2, Name = "Another org" });

            context.Users.Add(new DataModels.User() { UserId = 1, NameIdentifier = "auth|alice" });
            context.Users.Add(new DataModels.User() { UserId = 2, NameIdentifier = "auth|bob" });
            context.Users.Add(new DataModels.User() { UserId = 3, NameIdentifier = "auth|carol" });

            context.Jobs.Add(new DataModels.Job() { UserId = 1, OrganisationId = 1 });
            context.Jobs.Add(new DataModels.Job() { UserId = 2, OrganisationId = 1 });

            context.Jobs.Add(new DataModels.Job() { UserId = 2, OrganisationId = 2 });
            context.Jobs.Add(new DataModels.Job() { UserId = 3, OrganisationId = 2 });

            context.SaveChanges();

            //Act
            var orgAndUsers = userService.GetUsersAndJobIdsByOrganisationId(organisationId);

            //Assert
            orgAndUsers.OrganisationId.ShouldBe(1);
            orgAndUsers.UsersAndJobIds.Count.ShouldBe(2);
            orgAndUsers.UsersAndJobIds.FirstOrDefault().UserId.ShouldBe(1);
            orgAndUsers.UsersAndJobIds.FirstOrDefault().User.NameIdentifier.ShouldBe("auth|alice");

            orgAndUsers.UsersAndJobIds.LastOrDefault().UserId.ShouldBe(2);
            orgAndUsers.UsersAndJobIds.LastOrDefault().User.NameIdentifier.ShouldBe("auth|bob");
        }
    }
}
