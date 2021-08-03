using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using NICE.Identity.Authorisation.WebAPI.Environments;
using ApiModels = NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Test.Infrastructure;
using Shouldly;
using Xunit;

namespace NICE.Identity.Test.UnitTests.Authorisation.WebAPI.Services
{
    public class EnvironmentsServiceTests : TestBase 
    {
        private readonly Mock<ILogger<EnvironmentsService>> _logger;

        public EnvironmentsServiceTests()
        {
            _logger = new Mock<ILogger<EnvironmentsService>>();
        }

        [Fact]
        public void Create_environment()
        {
            //Arrange
            var context = GetContext();
            var environmentsService = new EnvironmentsService(context, _logger.Object);

            //Act
            var createdEnvironment = environmentsService.CreateEnvironment(new ApiModels.Environment()
            {
                Name = "Training",
                Order = 0
            });

            //Assert
            var environments = context.Environments.ToList();
            environments.Count.ShouldBe(1);
            environments.First().Name.ShouldBe("Training");
            createdEnvironment.Name.ShouldBe("Training");
        }

        [Fact]
        public void Get_environments_and_test_order()
        {
            //Arrange
            var context = GetContext();
            var environmentsService = new EnvironmentsService(context, _logger.Object);
            environmentsService.CreateEnvironment(new ApiModels.Environment()
            {
	            Name = "Training2",
	            Order = 2
            });
            environmentsService.CreateEnvironment(new ApiModels.Environment()
            {
                Name = "Training1",
                Order = 1
            });

            //Act
            var environments = environmentsService.GetEnvironments();

            //Assert
            environments.Count.ShouldBe(2);
            environments[0].Name.ShouldBe("Training1");
            environments[1].Name.ShouldBe("Training2");
        }
        
        [Fact]
        public void Get_environment()
        {
            //Arrange
            var context = GetContext();
            var environmentsService = new EnvironmentsService(context, _logger.Object);
            var createdEnvironmentId = environmentsService.CreateEnvironment(new ApiModels.Environment()
            {
                Name = "Training",
                Order = 0
            }).EnvironmentId;

            //Act
            var environment = environmentsService.GetEnvironment(createdEnvironmentId);

            //Assert
            environment.Name.ShouldBe("Training");
        }
        
        [Fact]
        public void Get_environment_that_does_not_exist()
        {
            //Arrange
            var context = GetContext();
            var environmentsService = new EnvironmentsService(context, _logger.Object);
            environmentsService.CreateEnvironment(new ApiModels.Environment()
            {
                Name = "Training",
                Order = 0
            });

            //Act
            var service = environmentsService.GetEnvironment(9999);

            //Assert
            service.ShouldBeNull();
        }

        [Fact]
        public void Update_environment()
        {
            //Arrange
            var context = GetContext();
            var environmentsService = new EnvironmentsService(context, _logger.Object);
            var createdEnvironmentId = environmentsService.CreateEnvironment(new ApiModels.Environment()
            {
                Name = "Training",
                Order = 0
            }).EnvironmentId;

            //Act
            var updatedEnvironment = environmentsService.UpdateEnvironment( createdEnvironmentId,
                new ApiModels.Environment()
                {
                    Name = "Training1", 
                    Order = 1
                });
            var environment = environmentsService.GetEnvironment(updatedEnvironment.EnvironmentId);

            //Assert
            updatedEnvironment.Name.ShouldBe("Training1");
            environment.Name.ShouldBe("Training1");
        }
        
        [Fact]
        public void Update_environment_that_does_not_exist()
        {
            //Arrange
            var context = GetContext();
            var environmentsService = new EnvironmentsService(context, _logger.Object);
            var environmentToUpdate = new ApiModels.Environment() {Name = "Updated Environment"};

            //Act + Assert
            Assert.Throws<Exception>(() => environmentsService.UpdateEnvironment(9999, environmentToUpdate));
            context.Environments.Count().ShouldBe(0);
        }
        
        [Fact]
        public void Delete_environment()
        {
            //Arrange
            var context = GetContext();
            var environmentsService = new EnvironmentsService(context, _logger.Object);
            var createdEnvironment = environmentsService.CreateEnvironment(new ApiModels.Environment()
            {
                Name = "Training",
                Order = 0
            });
            environmentsService.CreateEnvironment(new ApiModels.Environment()
            {
                Name = "Training2",
                Order = 1
            });

            //Act
            var deletedServiceResponse = environmentsService.DeleteEnvironment(createdEnvironment.EnvironmentId);

            //Assert
            deletedServiceResponse.ShouldBe(1);
            context.Environments.Count().ShouldBe(1);
        }

        [Fact]
        public void Delete_environment_that_does_not_exist()
        {
            //Arrange
            var context = GetContext();
            var environmentsService = new EnvironmentsService(context, _logger.Object);

            //Act
            var deletedEnvironmentResponse = environmentsService.DeleteEnvironment(9999);

            //Assert
            deletedEnvironmentResponse.ShouldBe(0);
        }
    }
}