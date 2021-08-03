using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Services;

namespace NICE.Identity.Authorisation.WebAPI.Controllers
{
    [Route("api/[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration)]
	[ApiController]
    public class WebsitesController: ControllerBase
    {
        private readonly ILogger<WebsitesController> _logger;
        private readonly IWebsitesService _websitesService;

        public WebsitesController(IWebsitesService websitesService, ILogger<WebsitesController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _websitesService = websitesService ?? throw new ArgumentNullException(nameof(websitesService));
        }
        
        /// <summary>
        /// create website
        /// </summary>
        /// <param name="website"></param>
        /// <returns></returns>
        [HttpPost("")]
        [ProducesResponseType(typeof(Website), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult CreateWebsite([FromBody] Website website)
        {
            if (!ModelState.IsValid)
            {
                var serializableModelState = new SerializableError(ModelState);
                var modelStateJson = JsonConvert.SerializeObject(serializableModelState);
                _logger.LogError($"Invalid model for create website", modelStateJson);
                return BadRequest(new ProblemDetails {Status = 400, Title = "Invalid model for create website"});
            }

            try
            {
                var createdWebsite = _websitesService.CreateWebsite(website);
                return Created($"/website/{createdWebsite.WebsiteId.ToString()}", createdWebsite);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails {Status = 500, Title = e.Message, Detail = e.ToString() });
            }
        }

        /// <summary>
        /// get list of all websites
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(List<Website>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetWebsites([FromQuery(Name = "q")] string filter)
        {
            try
            {
                return Ok(_websitesService.GetWebsites(filter));
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails {Status = 500, Title = $"{e.Message}"});
            }
        }

        /// <summary>
        /// get website with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Website), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetWebsite(int id)
        {
            try
            {
                var website = _websitesService.GetWebsite(id);
                if (website != null)
                {
                    return Ok(website);
                }
                return NotFound(new ProblemDetails {Status = 404, Title = "Website not found"});
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails {Status = 500, Title = $"{e.Message}"});
            }
        }

        /// <summary>
        /// update website with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("{id}", Name = "UpdateWebsitePartial")]
        [HttpPut("{id}", Name = "UpdateWebsite")]
        [ProducesResponseType(typeof(Website), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult UpdateWebsite([FromRoute] int id, [FromBody] Website website)
        {
            if (!ModelState.IsValid)
            {
                var serializableModelState = new SerializableError(ModelState);
                var modelStateJson = JsonConvert.SerializeObject(serializableModelState);
                _logger.LogError($"Invalid model for update website {modelStateJson}");
                return BadRequest(new ProblemDetails {Status = 400, Title = "Invalid model for update website"});
            }

            try
            {
                return Ok(_websitesService.UpdateWebsite(id, website));
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails {Status = 500, Title = $"{e.Message}"});
            }
        }
        /// <summary>
        /// delete website with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult DeleteWebsite(int id)
        {
            try
            {
                _websitesService.DeleteWebsite(id);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails {Status = 500, Title = $"{e.Message}"});
            }
        }
    }
}