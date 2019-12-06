using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Repositories;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
    public interface IServicesService
    {
        Service CreateService(Service service);
        Service GetService(int serviceId);
        List<Service> GetServices();
        Service UpdateService(int serviceId, Service service);
        int DeleteService(int serviceId);
    }
    
    public class ServicesService : IServicesService
    {
        private readonly IdentityContext _context;
        private readonly ILogger<ServicesService> _logger;
        public ServicesService(IdentityContext context, ILogger<ServicesService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Service CreateService(Service service)
        {
            try
            {
                var serviceToCreate = new DataModels.Service();
                serviceToCreate.UpdateFromApiModel(service);
                var createdService = _context.Services.Add(serviceToCreate);
                _context.SaveChanges();
                return new Service(createdService.Entity);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to create service {service.Name} - exception: {e.Message}");
                throw new Exception($"Failed to create service {service.Name} - exception: {e.Message}");
            }
        }

        public Service GetService(int serviceId)
        {
            var service = _context.Services
                .Include(s => s.Websites)
                .ThenInclude(w => w.Environment)
                .Where((s => s.ServiceId == serviceId))
                .FirstOrDefault();
            return service != null ? new Service(service) : null;
        }

        public List<Service> GetServices()
        {
            return _context.Services
                .Include(s => s.Websites)
                .ThenInclude(w => w.Environment)
                .Select(service => new Service(service))
                .ToList();
        }

        public Service UpdateService(int serviceId, Service service)
        {
            try
            {
                var serviceToUpdate = _context.Services.Find(serviceId);
                if (serviceToUpdate == null)
                    throw new Exception($"Service not found {serviceId.ToString()}");

                serviceToUpdate.UpdateFromApiModel(service);
                _context.SaveChanges();
                return new Service(serviceToUpdate);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to update service {serviceId.ToString()} - exception: {e.InnerException?.Message}");
                throw new Exception($"Failed to update service {serviceId.ToString()} - exception: {e.InnerException?.Message}");
            }
        }

        public int DeleteService(int serviceId)
        {
            try
            {
                var serviceToDelete = _context.Services.Find(serviceId);
                if (serviceToDelete == null)
                    return 0;
                _context.Services.RemoveRange(serviceToDelete);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to delete service {serviceId.ToString()} - exception: {e.Message}");
                throw new Exception($"Failed to delete service {serviceId.ToString()} - exception: {e.Message}");
            }
        }
    }
}
