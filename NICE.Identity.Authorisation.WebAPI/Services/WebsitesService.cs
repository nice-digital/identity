using System;
using System.Collections.Generic;
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
                return new Website(createdWebsite.Entity);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to create website. Exception: {e.Message}");
                throw new Exception($"Failed to create website. Exception: {e.Message}");
            }
        }

        public Website GetWebsite(int websiteId)
        {
            throw new System.NotImplementedException();
        }

        public List<Website> GetWebsites()
        {
            throw new System.NotImplementedException();
        }

        public Website UpdateWebsite(int websiteId, Website website)
        {
            throw new System.NotImplementedException();
        }

        public int DeleteWebsite(int websiteId)
        {
            throw new System.NotImplementedException();
        }
    }
}