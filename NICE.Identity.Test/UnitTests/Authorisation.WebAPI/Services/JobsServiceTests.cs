using Microsoft.Extensions.Logging;
using Moq;
using NICE.Identity.Authorisation.WebAPI.Services;
using NICE.Identity.Test.Infrastructure;
using Shouldly;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using ApiModels = NICE.Identity.Authorisation.WebAPI.ApiModels;


namespace NICE.Identity.Test.UnitTests.Authorisation.WebAPI.Services
{
    public class JobsServiceTests : TestBase
    {
        private readonly Mock<ILogger<JobsService>> _logger;

        public JobsServiceTests()
        {
            _logger = new Mock<ILogger<JobsService>>();
        }

        [Fact]
        public void Create_job()
        {
            //Arrange
            var context = GetContext();
            var jobsService = new JobsService(context, _logger.Object);
            TestData.AddUser(ref context);
            TestData.AddOrganisation(ref context);

            //Act
            var createdJob = jobsService.CreateJob(new ApiModels.Job()
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
            var jobsService = new JobsService(context, _logger.Object);
            TestData.AddUser(ref context);
            TestData.AddOrganisation(ref context);
            TestData.AddOrganisation(ref context, 2, "NHS");
            jobsService.CreateJob(new ApiModels.Job()
            {
                UserId = 1,
                OrganisationId = 1,
                IsLead = true
            });
            jobsService.CreateJob(new ApiModels.Job()
            {
                UserId = 1,
                OrganisationId = 2,
                IsLead = true
            });

            //Act
            var jobs = jobsService.GetJobs();

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
            var jobsService = new JobsService(context, _logger.Object);
            TestData.AddUser(ref context);
            TestData.AddOrganisation(ref context);
            var createdJob = jobsService.CreateJob(new ApiModels.Job()
            {
                UserId = 1,
                OrganisationId = 1,
                IsLead = true
            });

            //Act
            var job = jobsService.GetJob(createdJob.JobId.GetValueOrDefault());

            //Assert
            context.Jobs.Count().ShouldBe(1);
            job.JobId.ShouldBe(1);
        }

        [Fact]
        public void Get_job_that_does_not_exist()
        {
            //Arrange
            var context = GetContext();
            var jobsService = new JobsService(context, _logger.Object);
            TestData.AddUser(ref context);
            TestData.AddOrganisation(ref context);
            jobsService.CreateJob(new ApiModels.Job()
            {
                UserId = 1,
                OrganisationId = 1,
                IsLead = true
            });

            //Act
            var job = jobsService.GetJob(9999);

            //Assert
            job.OrganisationId.ShouldBe(null);
            job.UserId.ShouldBe(null);
            job.IsLead.ShouldBe(null);
        }

        [Fact]
        public void Update_job()
        {
            //Arrange
            var context = GetContext();
            var jobsService = new JobsService(context, _logger.Object);
            TestData.AddUser(ref context);
            TestData.AddOrganisation(ref context);
            var createdJobsId = jobsService.CreateJob(new ApiModels.Job
            {
                UserId = 1,
                OrganisationId = 1,
                IsLead = true,
            }).JobId.GetValueOrDefault();

            //Act
            var updatedOrganisation = jobsService.UpdateJob(createdJobsId, new ApiModels.Job()
            {
                UserId = 1,
                OrganisationId = 1,
                IsLead = false,
            });
            var job = jobsService.GetJob(createdJobsId);

            //Assert
            updatedOrganisation.IsLead.ShouldBe(false);
            job.IsLead.ShouldBe(false);
        }

        [Fact]
        public void Update_job_that_does_not_exist()
        {
            //Arrange
            var context = GetContext();
            var jobsService = new JobsService(context, _logger.Object);
            TestData.AddUser(ref context);
            TestData.AddOrganisation(ref context);
            var jobToUpdate = jobsService.CreateJob(new ApiModels.Job
            {
                UserId = 1,
                OrganisationId = 1,
                IsLead = true,
            }).JobId.GetValueOrDefault();

            //Act + Assert
            Assert.Throws<Exception>(() => jobsService.UpdateJob(9999, new ApiModels.Job()
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
            var jobsService = new JobsService(context, _logger.Object);
            TestData.AddUser(ref context);
            TestData.AddOrganisation(ref context);
            TestData.AddOrganisation(ref context, 2, "NHS");
            var createdJob = jobsService.CreateJob(new ApiModels.Job()
            {
                UserId = 1,
                OrganisationId = 1,
                IsLead = true
            });
            jobsService.CreateJob(new ApiModels.Job()
            {
                UserId = 1,
                OrganisationId = 2,
                IsLead = true
            });

            //Act
            var deletedJobResponse = jobsService.DeleteJob(createdJob.JobId.GetValueOrDefault());

            //Assert
            deletedJobResponse.ShouldBe(1);
            context.Jobs.Count().ShouldBe(1);
        }

        [Fact]
        public void Delete_job_that_does_not_exist()
        {
            //Arrange
            var context = GetContext();
            var jobsService = new JobsService(context, _logger.Object);
            TestData.AddUser(ref context);
            TestData.AddOrganisation(ref context);
            jobsService.CreateJob(new ApiModels.Job()
            {
                UserId = 1,
                OrganisationId = 1,
                IsLead = true
            });

            //Act
            var deletedJobResponse = jobsService.DeleteJob(9999);

            //Assert
            deletedJobResponse.ShouldBe(0);
            context.Jobs.Count().ShouldBe(1);
        }

        [Fact]
        public void Delete_All_Jobs_For_Organisation()
        {
            //Arrange
            var context = GetContext();
            var jobsService = new JobsService(context, _logger.Object);

            var deletedOrgId = 1;

            TestData.AddUser(ref context);
            TestData.AddOrganisation(ref context);
            jobsService.CreateJob(new ApiModels.Job() 
            {
                UserId = 1,
                OrganisationId = deletedOrgId,
                IsLead = true
            });

            jobsService.CreateJob(new ApiModels.Job()
            {
                UserId = 2,
                OrganisationId = deletedOrgId,
                IsLead = true
            });

            jobsService.CreateJob(new ApiModels.Job()
            {
                UserId = 1,
                OrganisationId = 2,
                IsLead = true
            });

            //Act
            jobsService.DeleteAllJobsForOrganisation(deletedOrgId);

            //Assert
            var modifiedCount = context.ChangeTracker.Entries().Count(x => x.State == EntityState.Deleted);

            modifiedCount.ShouldBe(2);
        }
    }
}
