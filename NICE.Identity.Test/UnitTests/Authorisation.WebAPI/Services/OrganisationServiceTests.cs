using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using NICE.Identity.Authorisation.WebAPI.Services;
using NICE.Identity.Test.Infrastructure;
using Shouldly;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Xunit;
using ApiModel = NICE.Identity.Authorisation.WebAPI.APIModels;

namespace NICE.Identity.Test.UnitTests.Authorisation.WebAPI.Services
{
    public class OrganisationServiceTests : TestBase
    {
        private readonly Mock<ILogger<OrganisationService>> _logger;

        public OrganisationServiceTests()
        {
            _logger = new Mock<ILogger<OrganisationService>>();
        }

        [Fact]
        public void Create_organisation()
        {
            //Arrange
            var context = GetContext();
            var organisationService = new OrganisationService(context, _logger.Object);

            //Act
            var createdOrganisation = organisationService.CreateOrganisation(new ApiModel.Organisation
            {
                OrganisationId = 1,
                Name = "Organisation",
            });

            //Assert
            var organisation = context.Organisations.ToList();
            organisation.Count.ShouldBe(1);
            organisation.First().Name.ShouldBe("Organisation");
        }

        [Fact]
        public void Get_Organisations()
        {
            //Arrange
            var context = GetContext();
            var OrganisationService = new OrganisationService(context, _logger.Object);

            //Act
            var organisation = OrganisationService.GetOrganisations();

            //Assert
        }
    }
}
