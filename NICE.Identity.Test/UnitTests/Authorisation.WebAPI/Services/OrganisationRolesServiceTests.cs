using Microsoft.Extensions.Logging;
using Moq;
using NICE.Identity.Authorisation.WebAPI.Services;
using NICE.Identity.Test.Infrastructure;
using Shouldly;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using ApiModels = NICE.Identity.Authorisation.WebAPI.ApiModels;


namespace NICE.Identity.Test.UnitTests.Authorisation.WebAPI.Services
{
    public class OrganisationRolesServiceTests : TestBase
    {
        private readonly Mock<ILogger<OrganisationRolesService>> _logger;

        public OrganisationRolesServiceTests()
        {
            _logger = new Mock<ILogger<OrganisationRolesService>>();
        }

        [Fact]
        public void Create_organisation_role()
        {
            //Arrange
            var context = GetContext();
            var organisationRolesService = new OrganisationRolesService(context, _logger.Object);
            TestData.AddOrganisation(ref context);
            TestData.AddRole(ref context);

            //Act
            var createdOrganisationRole = organisationRolesService.CreateOrganisationRole(new ApiModels.OrganisationRole()
            {
                OrganisationId = 1,
                RoleId = 1
            });

            //Assert
            var organisationRoles = context.OrganisationRoles.ToList();
            organisationRoles.Count.ShouldBe(1);
            organisationRoles.First().OrganisationId.ShouldBe(1);
            organisationRoles.First().RoleId.ShouldBe(1);
            createdOrganisationRole.OrganisationId.ShouldBe(1);
            createdOrganisationRole.RoleId.ShouldBe(1);
        }

        [Fact]
        public void Get_organisation_roles()
        {
            //Arrange
            var context = GetContext();
            var organisationRolesService = new OrganisationRolesService(context, _logger.Object);
            TestData.AddWithTwoRolesTwoOrganisations(ref context);
            organisationRolesService.CreateOrganisationRole(new ApiModels.OrganisationRole()
            {
                OrganisationId = 1,
                RoleId = 1
            });
            organisationRolesService.CreateOrganisationRole(new ApiModels.OrganisationRole()
            {
                OrganisationId = 2,
                RoleId = 2
            });

            //Act
            var organisationRoles = organisationRolesService.GetOrganisationRoles();

            //Assert
            organisationRoles.Count.ShouldBe(2);
            organisationRoles[0].OrganisationId.ShouldBe(1);
            organisationRoles[1].OrganisationId.ShouldBe(2);
        }

        [Fact]
        public void Get_organisation_role()
        {
            //Arrange
            var context = GetContext();
            var organisationRolesService = new OrganisationRolesService(context, _logger.Object);
            TestData.AddOrganisation(ref context);
            TestData.AddRole(ref context);
            var createdOrganisationRole = organisationRolesService.CreateOrganisationRole(new ApiModels.OrganisationRole()
            {
                OrganisationId = 1,
                RoleId = 1
            });

            //Act
            var organisationRole = organisationRolesService.GetOrganisationRole(createdOrganisationRole.OrganisationRoleId.GetValueOrDefault());

            //Assert
            context.OrganisationRoles.Count().ShouldBe(1);
            organisationRole.OrganisationId.ShouldBe(1);
        }

        [Fact]
        public void Get_organisation_role_that_does_not_exist()
        {
            //Arrange
            var context = GetContext();
            var organisationRolesService = new OrganisationRolesService(context, _logger.Object);
            TestData.AddOrganisation(ref context);
            TestData.AddRole(ref context);
            organisationRolesService.CreateOrganisationRole(new ApiModels.OrganisationRole()
            {
                OrganisationId = 1,
                RoleId = 1
            });

            //Act
            var organisationRole = organisationRolesService.GetOrganisationRole(9999);

            //Assert
            organisationRole.ShouldBeNull();
        }

        [Fact]
        public void Delete_organisation_role()
        {
            //Arrange
            var context = GetContext();
            var organisationRolesService = new OrganisationRolesService(context, _logger.Object);
            TestData.AddWithTwoRolesTwoOrganisations(ref context);
            var createdOrganisationRole = organisationRolesService.CreateOrganisationRole(new ApiModels.OrganisationRole()
            {
                OrganisationId = 1,
                RoleId = 1
            });
            organisationRolesService.CreateOrganisationRole(new ApiModels.OrganisationRole()
            {
                OrganisationId = 2,
                RoleId = 2
            });

            //Act
            var deletedOrganisationRoleResponse = organisationRolesService.DeleteOrganisationRole(createdOrganisationRole.OrganisationRoleId.GetValueOrDefault());

            //Assert
            deletedOrganisationRoleResponse.ShouldBe(1);
            context.OrganisationRoles.Count().ShouldBe(1);
        }

        [Fact]
        public void Delete_organisation_role_that_does_not_exist()
        {
            //Arrange
            var context = GetContext();
            var organisationRolesService = new OrganisationRolesService(context, _logger.Object);
            TestData.AddOrganisation(ref context);
            TestData.AddRole(ref context);
            organisationRolesService.CreateOrganisationRole(new ApiModels.OrganisationRole()
            {
                OrganisationId = 1,
                RoleId = 1
            });

            //Act
            var deletedOrganisationRoleResponse = organisationRolesService.DeleteOrganisationRole(9999);

            //Assert
            deletedOrganisationRoleResponse.ShouldBe(0);
            context.OrganisationRoles.Count().ShouldBe(1);
        }

        [Fact]
        public void Delete_All_Jobs_For_Organisation()
        {
            //Arrange
            var context = GetContext();
            var organisationRolesService = new OrganisationRolesService(context, _logger.Object);

            var deletedOrgId = 1;

            TestData.AddUser(ref context);
            TestData.AddOrganisation(ref context);
            organisationRolesService.CreateOrganisationRole(new ApiModels.OrganisationRole()
            {
                OrganisationId = deletedOrgId,
                RoleId = 1
            });

            organisationRolesService.CreateOrganisationRole(new ApiModels.OrganisationRole()
            {
                OrganisationId = deletedOrgId,
                RoleId = 2
            });

            organisationRolesService.CreateOrganisationRole(new ApiModels.OrganisationRole()
            {
                OrganisationId = 2,
                RoleId = 1
            });

            //Act
            organisationRolesService.DeleteAllOrganisationRolesForOrganisation(deletedOrgId);

            //Assert
            var modifiedCount = context.ChangeTracker.Entries().Count(x => x.State == EntityState.Deleted);

            modifiedCount.ShouldBe(2);
        }
    }
}
