using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
    public interface IUserJobsService
    {
        Job CreateJob(Job job);
        List<Job> GetJobs();
        Job GetJob(int jobId);
        Job UpdateJob(int jobId, Job job);
        int DeleteJob(int jobId);
    }

    public class UserJobsService : IUserJobsService
    {
        private readonly IdentityContext _context;
        private readonly ILogger<UserJobsService> _logger;
        public UserJobsService(IdentityContext context, ILogger<UserJobsService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Job CreateJob(Job job)
        {
            try
            {
                var jobToCreate = new DataModels.Job();
                jobToCreate.UpdateFromApiModel(job);
                var createdJob = _context.Jobs.Add(jobToCreate);
                _context.SaveChanges();
                return new Job(createdJob.Entity);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to create job for {job.JobId.ToString()} - exception: {e.Message}");
                throw new Exception($"Failed to create job for {job.JobId.ToString()} - exception: {e.Message}");
            }

         }

        public List<Job> GetJobs()
        {
            return _context.Jobs.Select(job => new Job(job)).ToList();
        }

        public Job GetJob(int jobId)
        {
            var job = _context.Jobs.Where((j => j.JobId == jobId)).FirstOrDefault();
            return job != null ? new Job(job) : null;
        }

        public Job UpdateJob(int jobId, Job job)
        {
            try
            {
                var jobToUpdate = _context.Jobs.Find(jobId);
                if (jobToUpdate == null)
                    throw new Exception($"Job not found {jobId.ToString()}");

                jobToUpdate.UpdateFromApiModel(job);
                _context.SaveChanges();
                return new Job(jobToUpdate);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to update job {jobId.ToString()} - exception: {e.Message}");
                throw new Exception($"Failed to update job {jobId.ToString()} - exception: {e.Message}");
            }
        }

        public int DeleteJob(int jobId)
        {
            try
            {
                var jobToDelete = _context.Jobs.Find(jobId);
                if (jobToDelete == null)
                    return 0;
                _context.Jobs.RemoveRange(jobToDelete);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to delete job {jobId.ToString()} - exception: {e.Message}");
                throw new Exception($"Failed to delete job {jobId.ToString()} - exception: {e.Message}");
            }
        }
    }

}
