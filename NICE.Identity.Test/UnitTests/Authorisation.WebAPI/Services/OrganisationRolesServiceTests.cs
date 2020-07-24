using Microsoft.Extensions.Logging;
using Moq;
using DataModels = NICE.Identity.Authorisation.WebAPI.DataModels;
using NICE.Identity.Authorisation.WebAPI.Services;
using NICE.Identity.Test.Infrastructure;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
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

            //Act
            var createdOrganisationRole = organisationRolesService.CreateOrganisationRole(new ApiModels.OrganisationRole()
            {
                OrganisationRoleId = 1,
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

    }
}
