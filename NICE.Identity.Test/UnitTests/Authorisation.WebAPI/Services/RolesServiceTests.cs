using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using NICE.Identity.Authorisation.WebAPI.Repositories;
using ApiModels = NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Services;
using NICE.Identity.Test.Infrastructure;
using Shouldly;
using Xunit;

namespace NICE.Identity.Test.UnitTests.Authorisation.WebAPI.Services
{
    public class RolesServiceTests : TestBase
    {
        private readonly Mock<ILogger<RolesService>> _logger;
        
        public RolesServiceTests()
        {
            _logger = new Mock<ILogger<RolesService>>();
        }

        [Fact]
        public void Create_role()
        {
            //Arrange
            var context = GetContext();
            var rolesService = new RolesService(context, _logger.Object);

            //Act
            var createdRole = rolesService.CreateRole(new ApiModels.Role()
            {
                WebsiteId = 1,
                Name = "Role",
                Description = "Role Description"
            });

            //Assert
            var roles = context.Roles.ToList();
            roles.Count.ShouldBe(1);
            roles.First().Name.ShouldBe("Role");
            createdRole.Name.ShouldBe("Role");
        }

        [Fact]
        public void Get_roles()
        {
            //Arrange
            var context = GetContext();
            var rolesService = new RolesService(context, _logger.Object);
            //TestData.AddEnvironment(ref context);
            //TestData.AddService(ref context);
            //TestData.AddWebsite(ref context);
            rolesService.CreateRole(new ApiModels.Role()
            {
                WebsiteId = 1,
                Name = "Role 1",
                Description = "Role Description 1"
            });
            rolesService.CreateRole(new ApiModels.Role()
            {
                WebsiteId = 1,
                Name = "Role 2",
                Description = "Role Description 2"
            });

            //Act
            var roles = rolesService.GetRoles();

            //Assert
            roles.Count.ShouldBe(2);
            roles[0].Name.ShouldBe("Role 1");
            roles[1].Name.ShouldBe("Role 2");
        }
        
        [Fact]
        public void Get_role()
        {
            //Arrange
            var context = GetContext();
            var rolesService = new RolesService(context, _logger.Object);
            TestData.AddEnvironment(ref context);
            TestData.AddService(ref context);
            TestData.AddWebsite(ref context);
            var createdRoleId = rolesService.CreateRole(new ApiModels.Role()
            {
                WebsiteId = 1,
                Name = "Role",
                Description = "Role Description"
            }).RoleId.GetValueOrDefault();
            
            //Act
            var role = rolesService.GetRole(createdRoleId);

            //Assert
            context.Roles.Count().ShouldBe(1);
            role.Name.ShouldBe("Role");
        }

        [Fact]
        public void Get_role_that_does_not_exist()
        {
            //Arrange
            var context = GetContext();
            var rolesService = new RolesService(context, _logger.Object);
            rolesService.CreateRole(new ApiModels.Role(){ Name = "Role" });

            //Act
            var role = rolesService.GetRole(9999);

            //Assert
            role.ShouldBeNull();
        }

        [Fact]
        public void Update_role()
        {
            //Arrange
            var context = GetContext();
            var rolesService = new RolesService(context, _logger.Object);
            var createdRoleId = rolesService.CreateRole(new ApiModels.Role()
            {
                WebsiteId = 1,
                Name = "Role",
                Description = "Role Description"
            }).RoleId.GetValueOrDefault();

            //Act
            var updatedRole = rolesService.UpdateRole(createdRoleId, new ApiModels.Role()
            {
                Name = "Role Updated",
            });
            var role = rolesService.GetRole(createdRoleId);

            //Assert
            updatedRole.Name.ShouldBe("Role Updated");
            role.Name.ShouldBe("Role Updated");
        }
        
        [Fact]
        public void Update_role_that_does_not_exist()
        {
            //Arrange
            var context = GetContext();
            var roleService = new RolesService(context, _logger.Object);
            var roleToUpdate = new ApiModels.Role() {Name = "Updated Role"};

            //Act + Assert
            Assert.Throws<Exception>(() => roleService.UpdateRole(9999, roleToUpdate));
        }
        
        [Fact]
        public void Delete_role()
        {
            //Arrange
            var context = GetContext();
            var rolesService = new RolesService(context, _logger.Object);
            TestData.AddEnvironment(ref context);
            TestData.AddService(ref context);
            TestData.AddWebsite(ref context);
            var createdRole = rolesService.CreateRole(new ApiModels.Role()
            {
                WebsiteId = 1,
                Name = "Role 1",
                Description = "Role Description 1"
            });
            rolesService.CreateRole(new ApiModels.Role()
            {
                WebsiteId = 1,
                Name = "Role 2",
                Description = "Role Description 2"
            });

            //Act
            var deletedRoleResponse = rolesService.DeleteRole(createdRole.RoleId.GetValueOrDefault());

            //Assert
            deletedRoleResponse.ShouldBe(1);
            context.Roles.Count().ShouldBe(1);
        }
    }
}
