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
    public class UserJobsServiceTests : TestBase
    {
        private readonly Mock<ILogger<UserJobsService>> _logger;

        public UserJobsServiceTests()
        {
            _logger = new Mock<ILogger<UserJobsService>>();
        }

        [Fact]
        public void Create_job()
        {
            //Arrange
            var context = GetContext();
            var userJobsService = new UserJobsService(context, _logger.Object);

            //Act
            var createdJob = userJobsService.CreateJob(new ApiModels.Job()
            {
                UserId = 1,
                OrganisationId = 1,
                IsLead = true
            });

            //Assert
            var jobs = context.Jobs.ToList();
            jobs.Count.ShouldBe(1);
            jobs.First().UserId.ShouldBe(1);
            jobs.First().OrganisationId.ShouldBe(1);
            jobs.First().IsLead.ShouldBe(true);
            createdJob.UserId.ShouldBe(1);
            createdJob.OrganisationId.ShouldBe(1);
            createdJob.IsLead.ShouldBe(true);
        }

        [Fact]
        public void Get_jobs()
        {
            //Arrange
            var context = GetContext();
            var userJobsService = new UserJobsService(context, _logger.Object);
            userJobsService.CreateJob(new ApiModels.Job()
            {
                UserId = 1,
                OrganisationId = 1,
                IsLead = true
            });
            userJobsService.CreateJob(new ApiModels.Job()
            {
                UserId = 1,
                OrganisationId = 2,
                IsLead = true
            });

            //Act
            var jobs = userJobsService.GetJobs();

            //Assert
            jobs.Count.ShouldBe(2);
            jobs[0].JobId.ShouldBe(1);
            jobs[1].JobId.ShouldBe(2);
        }

        [Fact]
        public void Get_job()
        {
            //Arrange
            var context = GetContext();
            var userJobsService = new UserJobsService(context, _logger.Object);
            var createdJob = userJobsService.CreateJob(new ApiModels.Job()
            {
                UserId = 1,
                OrganisationId = 1,
                IsLead = true
            });

            //Act
            var job = userJobsService.GetJob(createdJob.JobId.GetValueOrDefault());

            //Assert
            context.Jobs.Count().ShouldBe(1);
            job.JobId.ShouldBe(1);
        }

        [Fact]
        public void Get_job_that_does_not_exist()
        {
            //Arrange
            var context = GetContext();
            var userJobsService = new UserJobsService(context, _logger.Object);
            userJobsService.CreateJob(new ApiModels.Job()
            {
                UserId = 1,
                OrganisationId = 1,
                IsLead = true
            });

            //Act
            var job = userJobsService.GetJob(9999);

            //Assert
            job.ShouldBeNull();
        }

        [Fact]
        public void Update_job()
        {
            //Arrange
            var context = GetContext();
            var userJobsService = new UserJobsService(context, _logger.Object);
            var createdJobsId = userJobsService.CreateJob(new ApiModels.Job
            {
                UserId = 1,
                OrganisationId = 1,
                IsLead = true,
            }).JobId.GetValueOrDefault();

            //Act
            var updatedOrganisation = userJobsService.UpdateJob(createdJobsId, new ApiModels.Job()
            {
                UserId = 1,
                OrganisationId = 1,
                IsLead = false,
            });
            var job = userJobsService.GetJob(createdJobsId);

            //Assert
            updatedOrganisation.IsLead.ShouldBe(false);
            job.IsLead.ShouldBe(false);
        }

        [Fact]
        public void Update_job_that_does_not_exist()
        {
            //Arrange
            var context = GetContext();
            var userJobsService = new UserJobsService(context, _logger.Object);
            var jobToUpdate = userJobsService.CreateJob(new ApiModels.Job
            {
                UserId = 1,
                OrganisationId = 1,
                IsLead = true,
            }).JobId.GetValueOrDefault();

            //Act + Assert
            Assert.Throws<Exception>(() => userJobsService.UpdateJob(9999, new ApiModels.Job()
            {
                UserId = 1,
                OrganisationId = 1,
                IsLead = false,
            }));
        }

        [Fact]
        public void Delete_job()
        {
            //Arrange
            var context = GetContext();
            var userJobsService = new UserJobsService(context, _logger.Object);
            var createdJob = userJobsService.CreateJob(new ApiModels.Job()
            {
                UserId = 1,
                OrganisationId = 1,
                IsLead = true
            });
            userJobsService.CreateJob(new ApiModels.Job()
            {
                UserId = 1,
                OrganisationId = 2,
                IsLead = true
            });

            //Act
            var deletedJobResponse = userJobsService.DeleteJob(createdJob.JobId.GetValueOrDefault());

            //Assert
            deletedJobResponse.ShouldBe(1);
            context.Jobs.Count().ShouldBe(1);
        }

        [Fact]
        public void Delete_job_that_does_not_exist()
        {
            //Arrange
            var context = GetContext();
            var userJobsService = new UserJobsService(context, _logger.Object);
            userJobsService.CreateJob(new ApiModels.Job()
            {
                UserId = 1,
                OrganisationId = 1,
                IsLead = true
            });

            //Act
            var deletedJobResponse = userJobsService.DeleteJob(9999);

            //Assert
            deletedJobResponse.ShouldBe(0);
            context.Jobs.Count().ShouldBe(1);
        }
    }
}
