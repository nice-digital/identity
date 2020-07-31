using Microsoft.Extensions.Logging;
using Moq;
using NICE.Identity.Authorisation.WebAPI.Services;
using NICE.Identity.Test.Infrastructure;
using Shouldly;
using System;
using System.Linq;
using Xunit;
using ApiModels = NICE.Identity.Authorisation.WebAPI.ApiModels;

namespace NICE.Identity.Test.UnitTests.Authorisation.WebAPI.Services
{
    public class OrganisationsServiceTests : TestBase
    {
        private readonly Mock<ILogger<OrganisationsService>> _logger;

        public OrganisationsServiceTests()
        {
            _logger = new Mock<ILogger<OrganisationsService>>();
        }

        [Fact]
        public void Create_organisation()
        {
            //Arrange
            var context = GetContext();
            var organisationService = new OrganisationsService(context, _logger.Object);

            //Act
            var createdOrganisation = organisationService.CreateOrganisation(new ApiModels.Organisation
            {
                OrganisationId = 1,
                Name = "Organisation",
            });

            //Assert
            var organisation = context.Organisations.ToList();
            organisation.Count.ShouldBe(1);
            organisation.First().Name.ShouldBe("Organisation");
            createdOrganisation.Name.ShouldBe("Organisation");
        }

        [Fact]
        public void Get_Organisations()
        {
            //Arrange
            var context = GetContext();
            var organisationService = new OrganisationsService(context, _logger.Object);
            organisationService.CreateOrganisation(new ApiModels.Organisation
            {
                Name = "Organisation1",
            });
            organisationService.CreateOrganisation(new ApiModels.Organisation
            {
                Name = "Organisation2",
            });

            //Act
            var organisation = organisationService.GetOrganisations();

            //Assert
            organisation.Count.ShouldBe(2);
            organisation[0].Name.ShouldBe("Organisation1");
            organisation[1].Name.ShouldBe("Organisation2");
        }

        [Fact]
        public void Get_Organisation()
        {
            //Arrange
            var context = GetContext();
            var organisationService = new OrganisationsService(context, _logger.Object);
            var createdOrganisationId = organisationService.CreateOrganisation(new ApiModels.Organisation
            {
                Name = "Organisation",
            }).OrganisationId.GetValueOrDefault();

            //Act
            var organisation = organisationService.GetOrganisation(createdOrganisationId);

            //Assert
            context.Organisations.Count().ShouldBe(1);
            organisation.Name.ShouldBe("Organisation");
        }

        [Fact]
        public void Get_organisation_that_does_not_exist()
        {
            //Arrange
            var context = GetContext();
            var organisationService = new OrganisationsService(context, _logger.Object);
            organisationService.CreateOrganisation(new ApiModels.Organisation() { Name = "Organisation" });

            //Act
            var organisation = organisationService.GetOrganisation(9999);

            //Assert
            organisation.ShouldBeNull();
        }

        [Fact]
        public void Update_organisation()
        {
            //Arrange
            var context = GetContext();
            var organisationService = new OrganisationsService(context, _logger.Object);
            var createdOrganisationId = organisationService.CreateOrganisation(new ApiModels.Organisation
            {
                Name = "Organisation",
            }).OrganisationId.GetValueOrDefault();

            //Act
            var updatedOrganisation = organisationService.UpdateOrganisation(createdOrganisationId, new ApiModels.Organisation()
            {
                Name = "Organisation Updated",
            });
            var organisation = organisationService.GetOrganisation(createdOrganisationId);

            //Assert
            updatedOrganisation.Name.ShouldBe("Organisation Updated");
            organisation.Name.ShouldBe("Organisation Updated");
        }

        [Fact]
        public void Update_organisation_that_does_not_exist()
        {
            //Arrange
            var context = GetContext();
            var organisationService = new OrganisationsService(context, _logger.Object);
            var organisationToUpdate = new ApiModels.Organisation() { Name = "Updated Organisation" };

            //Act + Assert
            Assert.Throws<Exception>(() => organisationService.UpdateOrganisation(9999, organisationToUpdate));
        }

        [Fact]
        public void Delete_organisation()
        {
            //Arrange
            var context = GetContext();
            var organisationService = new OrganisationsService(context, _logger.Object);
            var createdOrganisation = organisationService.CreateOrganisation(new ApiModels.Organisation
            {
                Name = "Organisation1",
            });
            organisationService.CreateOrganisation(new ApiModels.Organisation
            {
                Name = "Organisation2",
            });

            //Act
            var deletedOrganisationResponse = organisationService.DeleteOrganisation(createdOrganisation.OrganisationId.GetValueOrDefault());

            //Assert
            deletedOrganisationResponse.ShouldBe(1);
            context.Organisations.Count().ShouldBe(1);
        }

        [Fact]
        public void Delete_organisation_that_does_not_exist()
        {
            //Arrange
            var context = GetContext();
            var organisationService = new OrganisationsService(context, _logger.Object);
            organisationService.CreateOrganisation(new ApiModels.Organisation
            {
                Name = "Organisation",
            });

            //Act
            var deletedOrganisationResponse = organisationService.DeleteOrganisation(9999);

            //Assert
            deletedOrganisationResponse.ShouldBe(0);
            context.Organisations.Count().ShouldBe(1);
        }
    }
}
