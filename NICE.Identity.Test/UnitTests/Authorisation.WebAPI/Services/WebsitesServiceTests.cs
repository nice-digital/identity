using Microsoft.Extensions.Logging;
using Moq;
using ApiModels = NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Services;
using NICE.Identity.Test.Infrastructure;
using Shouldly;
using System.Linq;
using Xunit;

namespace NICE.Identity.Test.UnitTests.Authorisation.WebAPI.Services
{
    public class WebsitesServiceTests : TestBase
    {
        private readonly Mock<ILogger<WebsitesService>> _logger;

        public WebsitesServiceTests()
        {
            _logger = new Mock<ILogger<WebsitesService>>();
        }

        [Fact]
        public void Create_website()
        {
            //Arrange
            var context = GetContext();
            var websitesService = new WebsitesService(context, _logger.Object);

            //Act
            websitesService.CreateWebsite(new ApiModels.Website()
            {
                ServiceId = 1,
                EnvironmentId = 1,
                Host = "test-host.nice.org.uk"
            });

            //Assert
            var websites = context.Websites.ToList();
            websites.First().Host.ShouldBe("test-host.nice.org.uk");
            websites.Count.ShouldBe(1);
        }
    }
}
