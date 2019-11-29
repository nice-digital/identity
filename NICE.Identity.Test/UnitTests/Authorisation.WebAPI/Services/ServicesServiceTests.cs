using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using ApiModels = NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Services;
using NICE.Identity.Test.Infrastructure;
using Shouldly;
using Xunit;

namespace NICE.Identity.Test.UnitTests.Authorisation.WebAPI.Services
{
    public class ServicesServiceTests : TestBase 
    {
        private readonly Mock<ILogger<ServicesService>> _logger;

        public ServicesServiceTests()
        {
            _logger = new Mock<ILogger<ServicesService>>();
        }

        [Fact]
        public void Create_service()
        {
            //Arrange
            var context = GetContext();
            var serviceService = new ServicesService(context, _logger.Object);

            //Act
            var createdService = serviceService.CreateService(new ApiModels.Service()
            {
                Name = "Test Service"
            });

            //Assert
            var services = context.Services.ToList();
            services.Count.ShouldBe(1);
            services.First().Name.ShouldBe("Test Service");
            createdService.Name.ShouldBe("Test Service");
        }

        [Fact]
        public void Get_services()
        {
            //Arrange
            var context = GetContext();
            var serviceService = new ServicesService(context, _logger.Object);
            serviceService.CreateService(new ApiModels.Service()
            {
                Name = "Service 1"
            });
            serviceService.CreateService(new ApiModels.Service()
            {
                Name = "Service 2"
            });

            //Act
            var services = serviceService.GetServices();

            //Assert
            services.Count.ShouldBe(2);
            services[0].Name.ShouldBe("Service 1");
            services[1].Name.ShouldBe("Service 2");
        }
        
        [Fact]
        public void Get_service()
        {
            //Arrange
            var context = GetContext();
            var serviceService = new ServicesService(context, _logger.Object);
            var createdServiceId = serviceService.CreateService(new ApiModels.Service()
            {
                Name = "Service 1"
            }).ServiceId.GetValueOrDefault();

            //Act
            var service = serviceService.GetService(createdServiceId);

            //Assert
            service.Name.ShouldBe("Service 1");
        }
        
        [Fact]
        public void Get_service_that_does_not_exist()
        {
            //Arrange
            var context = GetContext();
            var serviceService = new ServicesService(context, _logger.Object);
            serviceService.CreateService(new ApiModels.Service(){ Name = "Service 1" });

            //Act
            var service = serviceService.GetService(9999);

            //Assert
            service.ShouldBeNull();
        }

        [Fact]
        public void Update_service()
        {
            //Arrange
            var context = GetContext();
            var serviceService = new ServicesService(context, _logger.Object);
            var createdServiceId = serviceService.CreateService(new ApiModels.Service()
            {
                Name = "Service 1",
            }).ServiceId.GetValueOrDefault();

            //Act
            var updatedService = serviceService.UpdateService(createdServiceId, new ApiModels.Service()
            {
                Name = "Service 2"
            });
            var service = serviceService.GetService(updatedService.ServiceId.GetValueOrDefault());

            //Assert
            updatedService.Name.ShouldBe("Service 2");
            service.Name.ShouldBe("Service 2");
        }
        
        [Fact]
        public void Update_service_that_does_not_exist()
        {
            //Arrange
            var context = GetContext();
            var serviceService = new ServicesService(context, _logger.Object);
            var serviceToUpdate = new ApiModels.Service() {Name = "Updated Service"};

            //Act + Assert
            Assert.Throws<Exception>(() => serviceService.UpdateService(9999, serviceToUpdate));
            context.Services.Count().ShouldBe(0);
        }
        
        [Fact]
        public void Delete_service()
        {
            //Arrange
            var context = GetContext();
            var servicesService = new ServicesService(context, _logger.Object);
            var createdService = servicesService.CreateService(new ApiModels.Service()
            {
                Name = "Service 1"
            });
            servicesService.CreateService(new ApiModels.Service()
            {
                Name = "Service 2"
            });

            //Act
            var deletedServiceResponse = servicesService.DeleteService(createdService.ServiceId.GetValueOrDefault());

            //Assert
            deletedServiceResponse.ShouldBe(1);
            context.Services.Count().ShouldBe(1);
        }

        [Fact]
        public void Delete_service_that_does_not_exist()
        {
            //Arrange
            var context = GetContext();
            var servicesService = new ServicesService(context, _logger.Object);

            //Act
            var deletedServiceResponse = servicesService.DeleteService(9999);

            //Assert
            deletedServiceResponse.ShouldBe(0);
        }
    }
}