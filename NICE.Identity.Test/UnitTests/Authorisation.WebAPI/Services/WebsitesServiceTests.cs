using System;
using Microsoft.Extensions.Logging;
using Moq;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Services;
using NICE.Identity.Test.Infrastructure;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using DataModels = NICE.Identity.Authorisation.WebAPI.DataModels;

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
            websites[0].Service.Name.ShouldBe("Test Service");
        }

        [Fact]
        public void Get_websites_with_filters()
        {
            //Arrange
            var context = GetContext();
            var websitesService = new WebsitesService(context, _logger.Object);
            TestData.AddService(ref context, 1, "Test Service");
            TestData.AddService(ref context, 2, "Other Service");
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
            websitesService.CreateWebsite(new Website()
            {
                ServiceId = 2,
                EnvironmentId = 2,
                Host = "test3-host.nice.org.uk"
            });

            //Act
            var websitesFilterByServiceName = websitesService.GetWebsites("Test Service");
            var websitesFilterByHostName = websitesService.GetWebsites("test2");

            //Assert
            websitesFilterByServiceName.Count.ShouldBe(2);
            websitesFilterByServiceName[0].Host.ShouldBe("test1-host.nice.org.uk");
            websitesFilterByServiceName[1].Host.ShouldBe("test2-host.nice.org.uk");

            websitesFilterByHostName.Count.ShouldBe(1);
            websitesFilterByHostName[0].Host.ShouldBe("test2-host.nice.org.uk");
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

        [Fact]
        public async Task Get_Users_And_Roles_For_Website()
        {
            //Arrange
            var websiteId = 1;
            var context = GetContext();
            var websitesService = new WebsitesService(context, _logger.Object);

            context.Users.Add(new DataModels.User() { NameIdentifier = "user1", EmailAddress = "first.email@example.com" });
            context.Users.Add(new DataModels.User() { NameIdentifier = "user2", EmailAddress = "second.email@example.com" });
            context.Environments.Add(new DataModels.Environment() { EnvironmentId = 1, Name = "Test" });
            context.Services.Add(new DataModels.Service() { ServiceId = 1, Name = "Service" });
            context.Websites.Add(new DataModels.Website() { WebsiteId = 1, EnvironmentId = 1, ServiceId = 1, Host = "test.nice.org.uk"});
            context.Roles.Add(new DataModels.Role() { RoleId = 1, WebsiteId = 1, Name = "TestRole1" });
            context.Roles.Add(new DataModels.Role() { RoleId = 2, WebsiteId = 1, Name = "TestRole2" });
            context.UserRoles.Add(new DataModels.UserRole() { RoleId = 1, UserId = 1 });
            context.UserRoles.Add(new DataModels.UserRole() { RoleId = 2, UserId = 1 });
            context.UserRoles.Add(new DataModels.UserRole() { RoleId = 2, UserId = 2 });
            context.SaveChanges();

            //Act
            var usersAndRoles = websitesService.GetRolesAndUsersForWebsite(websiteId);

            //Assert
            usersAndRoles.Count().ShouldBe(2);

            usersAndRoles.First().NameIdentifier.ShouldBe("user1");
            usersAndRoles.First().Roles.Count().ShouldBe(2);
            usersAndRoles.First().Roles.First().Name.ShouldBe("TestRole1");
            usersAndRoles.First().Roles.Last().Name.ShouldBe("TestRole2");

            usersAndRoles.Last().NameIdentifier.ShouldBe("user2");
            usersAndRoles.Last().Roles.Count().ShouldBe(1);
            usersAndRoles.Last().Roles.First().Name.ShouldBe("TestRole2");
        }
    }
}
