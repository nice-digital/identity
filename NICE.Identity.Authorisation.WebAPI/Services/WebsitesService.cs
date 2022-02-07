using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.APIModels;
using NICE.Identity.Authorisation.WebAPI.Repositories;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
    public interface IWebsitesService
    {
        Website CreateWebsite(Website website);
        Website GetWebsite(int websiteId);
        List<Website> GetWebsites(string filter);
        Website UpdateWebsite(int websiteId, Website website);
        int DeleteWebsite(int websiteId);
        UserAndRoleByWebsite GetRolesAndUsersForWebsite(int websiteId);
    }

    public class WebsitesService: IWebsitesService
    {
        private IdentityContext _context;
        private ILogger<WebsitesService> _logger;
        public WebsitesService(IdentityContext context, ILogger<WebsitesService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public Website CreateWebsite(Website website)
        {
            try
            {
                var websiteToCreate = new DataModels.Website();
                websiteToCreate.UpdateFromApiModel(website);
                var createdWebsite = _context.Websites.Add(websiteToCreate);
                _context.SaveChanges();
                var returnWebsite = _context.Websites
                    .Include(w => w.Environment)
                    .Where((w => w.WebsiteId == createdWebsite.Entity.WebsiteId))
                    .FirstOrDefault();
                return new Website(returnWebsite);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to create website. Exception: {e.Message}");
                throw new Exception($"Failed to create website. Exception: {e.Message}");
            }
        }

        public List<Website> GetWebsites(string filter = null)
        {
            return _context.FindWebsites(filter)
                .Select(website => new Website(website))
                .ToList(); ;
        }

        public Website GetWebsite(int websiteId)
        {
            var website = _context.GetWebsite(websiteId);
            return website != null ? new Website(website) : null;
        }

        public Website UpdateWebsite(int websiteId, Website website)
        {
            try
            {
                var websiteToUpdate = _context.Websites.Find(websiteId);
                if (websiteToUpdate == null)
                    throw new Exception($"Website not found {websiteId.ToString()}");

                websiteToUpdate.UpdateFromApiModel(website);
                _context.SaveChanges();
                return new Website(_context.GetWebsite(websiteId));
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to update website {websiteId.ToString()} - exception: {e.InnerException.Message}");
                throw new Exception($"Failed to update website {websiteId.ToString()} - exception: {e.InnerException.Message}");
            }
        }

        public int DeleteWebsite(int websiteId)
        {
            try
            {
                var websiteToDelete = _context.Websites.Find(websiteId);
                if (websiteToDelete == null)
                    return 0;
                _context.Websites.RemoveRange(websiteToDelete);
                return _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to delete website {websiteId.ToString()} - exception: {e.Message}");
                throw new Exception($"Failed to delete website {websiteId.ToString()} - exception: {e.Message}");
            }
        }

        /// <summary>
        /// Gets a list of users and their roles for a website
        /// </summary>
        /// <param name="websiteId"></param>
        /// <returns></returns>
        public UserAndRoleByWebsite GetRolesAndUsersForWebsite(int websiteId)
        {
            var website = _context.Websites
                .Include(w => w.Service)
                .Include(w => w.Environment)
                .FirstOrDefault(w => w.WebsiteId == websiteId);

            if (website == null)
                return null;

            var roles = _context.Roles
                .Where(r => r.WebsiteId == websiteId)
                .ToList();

            var rolesForWebsite = roles.Select(r => r.RoleId).ToList();
            
            var users = _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(u => u.Role)
                .Where(u => u.UserRoles.Any(ur => rolesForWebsite.Contains(ur.RoleId)));

            var userAndRoles = new List<UserAndRoles>();
            foreach (var user in users)
            {
                var userRoleDetailed = new List<UserRoleDetailed>();
                foreach (var role in user.UserRoles)
                {
                    if (rolesForWebsite.Contains(role.Role.RoleId))
                    {
                        userRoleDetailed.Add(new UserRoleDetailed(role.Role.RoleId, true, role.Role.Name, role.Role.Description));
                    }
                }

                userAndRoles.Add(new UserAndRoles(user.UserId, new User(user), userRoleDetailed));
            }
            
            var userAndRoleByWebsite = new UserAndRoleByWebsite()
            {
                WebsiteId = website.WebsiteId,
                ServiceId = website.ServiceId,
                EnvironmentId = website.EnvironmentId,
                UsersAndRoles = userAndRoles,
                Website = new Website(website),
                Service = new Service()
                {
                    ServiceId = website.Service.ServiceId,
                    Name = website.Service.Name
                },
                Environment = new ApiModels.Environment()
                {
                    EnvironmentId = website.Environment.EnvironmentId,
                    Name = website.Environment.Name
                }
            };

            return userAndRoleByWebsite;
        }
    }
}