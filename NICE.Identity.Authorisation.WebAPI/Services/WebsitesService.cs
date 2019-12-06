using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Repositories;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
    public interface IWebsitesService
    {
        Website CreateWebsite(Website website);
        Website GetWebsite(int websiteId);
        List<Website> GetWebsites();
        Website UpdateWebsite(int websiteId, Website website);
        int DeleteWebsite(int websiteId);
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

        public List<Website> GetWebsites()
        {
            return _context.GetWebsites().ConvertAll(website => new Website(website));
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
    }
}