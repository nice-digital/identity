using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.Repositories;
using Environment = NICE.Identity.Authorisation.WebAPI.ApiModels.Environment;

namespace NICE.Identity.Authorisation.WebAPI.Environments
{
    public interface IEnvironmentsService
    {
        Environment CreateEnvironment(Environment environment);
        Environment GetEnvironment(int environmentId);
        List<Environment> GetEnvironments();
        Environment UpdateEnvironment(int environmentId, Environment environment);
        int DeleteEnvironment(int environmentId);
    }
    
    public class EnvironmentsService : IEnvironmentsService
    {
        private readonly IdentityContext _context;
        private readonly ILogger<EnvironmentsService> _logger;
        public EnvironmentsService(IdentityContext context, ILogger<EnvironmentsService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Environment CreateEnvironment(Environment environment)
        {
            try
            {
                var environmentToCreate = new DataModels.Environment();
                environmentToCreate.UpdateFromApiModel(environment);
                var createdEnvironment = _context.Environments.Add(environmentToCreate);
                _context.SaveChanges();
                return new Environment(createdEnvironment.Entity);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to create environment {environment.Name} - exception: {e.Message}");
                throw new Exception($"Failed to create environment {environment.Name} - exception: {e.Message}");
            }
        }

        public Environment GetEnvironment(int environmentId)
        {
            var environment = _context.Environments.Where((e => e.EnvironmentId == environmentId)).FirstOrDefault();
            return environment != null ? new Environment(environment) : null;
        }

        public List<Environment> GetEnvironments()
        {
            return _context.Environments.Select(e => new Environment(e)).ToList();
        }

        public Environment UpdateEnvironment(int environmentId, Environment environment)
        {
            try
            {
                var environmentToUpdate = _context.Environments.Find(environmentId);
                if (environmentToUpdate == null)
                    throw new Exception($"Environment not found {environmentId.ToString()}");

                environmentToUpdate.UpdateFromApiModel(environment);
                _context.SaveChanges();
                return new Environment(environmentToUpdate);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to update environment {environmentId.ToString()} - exception: {e.InnerException?.Message}");
                throw new Exception($"Failed to update environment {environmentId.ToString()} - exception: {e.InnerException?.Message}");
            }
        }

        public int DeleteEnvironment(int environmentId)
        {
            try
            {
                var environmentToDelete = _context.Environments.Find(environmentId);
                if (environmentToDelete == null)
                    return 0;
                _context.Environments.RemoveRange(environmentToDelete);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to delete environment {environmentId.ToString()} - exception: {e.Message}");
                throw new Exception($"Failed to delete environment {environmentId.ToString()} - exception: {e.Message}");
            }
        }
    }
}
