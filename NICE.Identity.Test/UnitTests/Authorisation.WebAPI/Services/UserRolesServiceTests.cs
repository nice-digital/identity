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
    public class UserRolesServiceTests : TestBase
    {
        private readonly Mock<ILogger<UserRolesService>> _logger;

        public UserRolesServiceTests()
        {
            _logger = new Mock<ILogger<UserRolesService>>();
        }

        [Fact]
        public void Create_user_role()
        {
            //Arrange
            var context = GetContext();
            var userRolesService = new UserRolesService(context, _logger.Object);

            //Act
            var createdUserRole = userRolesService.CreateUserRole(new ApiModels.UserRole()
            {
                RoleId = 1,
                UserId = 1
            });

            //Assert
            var userRoles = context.UserRoles.ToList();
            userRoles.Count.ShouldBe(1);
            userRoles.First().RoleId.ShouldBe(1);
            userRoles.First().UserId.ShouldBe(1);
            createdUserRole.RoleId.ShouldBe(1);
            createdUserRole.UserId.ShouldBe(1);
        }

        [Fact]
        public void Get_user_roles()
        {
            //Arrange
            var context = GetContext();
            var userRolesService = new UserRolesService(context, _logger.Object);
            userRolesService.CreateUserRole(new ApiModels.UserRole()
            {
                RoleId = 1,
                UserId = 1
            });
            userRolesService.CreateUserRole(new ApiModels.UserRole()
            {
                RoleId = 1,
                UserId = 2
            });

            //Act
            var userRoles = userRolesService.GetUserRoles();

            //Assert
            userRoles.Count.ShouldBe(2);
            userRoles[0].UserId.ShouldBe(1);
            userRoles[1].UserId.ShouldBe(2);
        }
        
        [Fact]
        public void Get_user_role()
        {
            //Arrange
            var context = GetContext();
            var userRolesService = new UserRolesService(context, _logger.Object);
            var createdUserRoleId = userRolesService.CreateUserRole(new ApiModels.UserRole()
            {
                RoleId = 1,
                UserId = 1
            }).UserRoleId.GetValueOrDefault();
            
            //Act
            var userRole = userRolesService.GetUserRole(createdUserRoleId);

            //Assert
            context.UserRoles.Count().ShouldBe(1);
            userRole.UserId.ShouldBe(1);
        }

        [Fact]
        public void Delete_user_role()
        {
            //Arrange
            var context = GetContext();
            var userRolesService = new UserRolesService(context, _logger.Object);
            TestData.AddEnvironment(ref context);
            TestData.AddService(ref context);
            TestData.AddWebsite(ref context);
            var createdUserRole = userRolesService.CreateUserRole(new ApiModels.UserRole()
            {
                RoleId = 1,
                UserId = 1
            });
            userRolesService.CreateUserRole(new ApiModels.UserRole()
            {
                RoleId = 1,
                UserId = 2
            });

            //Act
            var deletedUserRoleResponse = userRolesService.DeleteRole(createdUserRole.UserRoleId.GetValueOrDefault());

            //Assert
            deletedUserRoleResponse.ShouldBe(1);
            context.UserRoles.Count().ShouldBe(1);
        }
    }
}
