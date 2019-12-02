using System;
using Microsoft.Extensions.Logging;
using Moq;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
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
            TestData.AddService(ref context, 1, "Test Service");
            TestData.AddEnvironment(ref context, 1, "Dev");

            //Act
            var createdWebsite = websitesService.CreateWebsite(new Website()
            {
                ServiceId = 1,
                EnvironmentId = 1,
                Host = "test-host.nice.org.uk"
            });

            //Assert
            var websites = context.Websites.ToList();
            websites.Count.ShouldBe(1);
            websites.First().Host.ShouldBe("test-host.nice.org.uk");
            createdWebsite.Host.ShouldBe("test-host.nice.org.uk");
        }

        [Fact]
        public void Get_websites()
        {
            //Arrange
            var context = GetContext();
            var websitesService = new WebsitesService(context, _logger.Object);
            TestData.AddService(ref context, 1, "Test Service");
            TestData.AddEnvironment(ref context, 1, "Dev");
            TestData.AddEnvironment(ref context, 2, "Test");
            websitesService.CreateWebsite(new Website()
            {
                ServiceId = 1,
                EnvironmentId = 1,
                Host = "test1-host.nice.org.uk"
            });
            websitesService.CreateWebsite(new Website()
            {
                ServiceId = 1,
                EnvironmentId = 2,
                Host = "test2-host.nice.org.uk"
            });

            //Act
            var websites = websitesService.GetWebsites();

            //Assert
            websites.Count.ShouldBe(2);
            websites[0].Host.ShouldBe("test1-host.nice.org.uk");
            websites[1].Host.ShouldBe("test2-host.nice.org.uk");
        }
        
        [Fact]
        public void Get_website()
        {
            //Arrange
            var context = GetContext();
            var websitesService = new WebsitesService(context, _logger.Object);
            TestData.AddService(ref context, 1, "Test Service");
            TestData.AddEnvironment(ref context, 1, "Dev");
            var createdWebsiteId = websitesService.CreateWebsite(new Website()
            {
                ServiceId = 1,
                EnvironmentId = 1,
                Host = "test-host.nice.org.uk"
            }).WebsiteId.GetValueOrDefault();

            //Act
            var website = websitesService.GetWebsite(createdWebsiteId);

            //Assert
            website.ServiceId.ShouldBe(1);
            website.EnvironmentId.ShouldBe(1);
            website.Host.ShouldBe("test-host.nice.org.uk");
        }
        
        [Fact]
        public void Get_website_that_does_not_exist()
        {
            //Arrange
            var context = GetContext();
            var websitesService = new WebsitesService(context, _logger.Object);

            //Act
            var website = websitesService.GetWebsite(9999);

            //Assert
            website.ShouldBeNull();
        }

        [Fact]
        public void Update_website()
        {
            //Arrange
            var context = GetContext();
            var websitesService = new WebsitesService(context, _logger.Object);
            TestData.AddService(ref context, 1, "Test Service1");
            TestData.AddService(ref context, 2, "Test Service2");
            TestData.AddEnvironment(ref context, 1, "Dev");
            TestData.AddEnvironment(ref context, 2, "Test");
            var createdWebsiteId = websitesService.CreateWebsite(new Website()
            {
                ServiceId = 1,
                EnvironmentId = 1,
                Host = "test-host.nice.org.uk"
            }).WebsiteId.GetValueOrDefault();

            //Act
            var updatedWebsite = websitesService.UpdateWebsite(createdWebsiteId, new Website()
            {
                ServiceId = 2,
                EnvironmentId = 2,
                Host = "updated-host.nice.org.uk"
            });
            var website = websitesService.GetWebsite(updatedWebsite.WebsiteId.GetValueOrDefault());

            //Assert
            updatedWebsite.ServiceId.ShouldBe(2);
            updatedWebsite.EnvironmentId.ShouldBe(2);
            updatedWebsite.Host.ShouldBe("updated-host.nice.org.uk");
            website.Host.ShouldBe("updated-host.nice.org.uk");
        }

        [Fact]
        public void Update_website_partial()
        {
            //Arrange
            var context = GetContext();
            var websitesService = new WebsitesService(context, _logger.Object);
            TestData.AddService(ref context, 1, "Test Service1");
            TestData.AddEnvironment(ref context, 1, "Dev");
            var createdWebsiteId = websitesService.CreateWebsite(new Website()
            {
                ServiceId = 1,
                EnvironmentId = 1,
                Host = "test-host.nice.org.uk"
            }).WebsiteId.GetValueOrDefault();

            //Act
            var partiallyUpdatedWebsite = websitesService.UpdateWebsite(createdWebsiteId, new Website()
            {
                Host = "updated-host.nice.org.uk"
            });
            var website = websitesService.GetWebsite(partiallyUpdatedWebsite.WebsiteId.GetValueOrDefault());

            //Assert
            partiallyUpdatedWebsite.ServiceId.ShouldBe(1);
            partiallyUpdatedWebsite.EnvironmentId.ShouldBe(1);
            partiallyUpdatedWebsite.Host.ShouldBe("updated-host.nice.org.uk");
            website.Host.ShouldBe("updated-host.nice.org.uk");
        }
        
        [Fact]
        public void Update_website_that_does_not_exist()
        {
            //Arrange
            var context = GetContext();
            var websitesService = new WebsitesService(context, _logger.Object);
            var websiteToUpdate = new Website() {Host = "updated-host.nice.org.uk"};

            //Act + Assert
            Assert.Throws<NullReferenceException>(() => websitesService.UpdateWebsite(9999, websiteToUpdate));
            context.Websites.Count().ShouldBe(0);
        }
        
        [Fact]
        public void Delete_website()
        {
            //Arrange
            var context = GetContext();
            var websitesService = new WebsitesService(context, _logger.Object);
            TestData.AddService(ref context, 1, "Test Service1");
            TestData.AddEnvironment(ref context, 1, "Dev");
            var createdWebsite = websitesService.CreateWebsite(new Website()
            {
                ServiceId = 1,
                EnvironmentId = 1,
                Host = "test1-host.nice.org.uk"
            });
            TestData.AddService(ref context, 2, "Test Service2");
            TestData.AddEnvironment(ref context, 2, "Test");
            websitesService.CreateWebsite(new Website()
            {
                ServiceId = 2,
                EnvironmentId = 2,
                Host = "test2-host.nice.org.uk"
            });

            //Act
            var deletedWebsiteResponse = websitesService.DeleteWebsite(createdWebsite.WebsiteId.GetValueOrDefault());

            //Assert
            deletedWebsiteResponse.ShouldBe(1);
            context.Websites.Count().ShouldBe(1);
        }

        [Fact]
        public void Delete_website_that_does_not_exist()
        {
            //Arrange
            var context = GetContext();
            var websitesService = new WebsitesService(context, _logger.Object);

            //Act
            var deletedWebsiteResponse = websitesService.DeleteWebsite(9999);

            //Assert
            deletedWebsiteResponse.ShouldBe(0);
        }
    }
}
