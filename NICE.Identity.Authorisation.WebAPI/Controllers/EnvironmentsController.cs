using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NICE.Identity.Authorisation.WebAPI.Environments;
using Environment = NICE.Identity.Authorisation.WebAPI.ApiModels.Environment;

namespace NICE.Identity.Authorisation.WebAPI.Controllers
{

    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class EnvironmentsController : ControllerBase
    {
        private readonly ILogger<EnvironmentsController> _logger;
        private readonly IEnvironmentsService _environmentsService;
        
        public EnvironmentsController(IEnvironmentsService environmentsService, ILogger<EnvironmentsController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _environmentsService = environmentsService ?? throw new ArgumentNullException(nameof(environmentsService));
        }
        
        /// <summary>
        /// create environment
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        [HttpPost("")]
        [ProducesResponseType(typeof(Environment), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult CreateEnvironment(Environment environment)
        {
            if (!ModelState.IsValid)
            {
                var serializableModelState = new SerializableError(ModelState);
                var modelStateJson = JsonConvert.SerializeObject(serializableModelState);
                _logger.LogError($"Invalid model for create environment", modelStateJson);
                return BadRequest(new ProblemDetails {Status = 400, Title = "Invalid model for create environment"});
            }

            try
            {
                var createdEnvironment = _environmentsService.CreateEnvironment(environment);
                return Created($"/environment/{createdEnvironment.EnvironmentId.ToString()}", createdEnvironment);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails {Status = 500, Title = e.Message, Detail = e.InnerException?.Message});
            }
        }

        /// <summary>
        /// get list of all environments
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(List<Environment>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetEnvironments()
        {
            try
            {
                return Ok(_environmentsService.GetEnvironments());
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails {Status = 500, Title = $"{e.Message}"});
            }
        }

        /// <summary>
        /// get environment with id
        /// </summary>
        /// <param name="environmentId"></param>
        /// <returns></returns>
        [HttpGet("{environmentId}")]
        [ProducesResponseType(typeof(Environment), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetEnvironment(int environmentId)
        {
            try
            {
                var environment = _environmentsService.GetEnvironment(environmentId);
                if (environment != null)
                {
                    return Ok(environment);
                }
                return NotFound(new ProblemDetails {Status = 404, Title = "Environment not found"});
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails {Status = 500, Title = $"{e.Message}"});
            }
        }

        /// <summary>
        /// update environment with id
        /// </summary>
        /// <param name="environmentId"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        [HttpPatch("{environmentId}", Name = "UpdateEnvironmentPartial")]
        [HttpPut("{environmentId}", Name = "UpdateEnvironment")]
        [ProducesResponseType(typeof(Environment), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult UpdateEnvironment(int environmentId, Environment environment)
        {
            if (!ModelState.IsValid)
            {
                var serializableModelState = new SerializableError(ModelState);
                var modelStateJson = JsonConvert.SerializeObject(serializableModelState);
                _logger.LogError($"Invalid model for update environment {modelStateJson}");
                return BadRequest(new ProblemDetails {Status = 400, Title = "Invalid model for update environment"});
            }

            try
            {
                return Ok(_environmentsService.UpdateEnvironment(environmentId, environment));
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails {Status = 500, Title = $"{e.Message}"});
            }
        }
        /// <summary>
        /// delete environment with id
        /// </summary>
        /// <param name="environmentId"></param>
        /// <returns></returns>
        [HttpDelete("{environmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult DeleteEnvironment(int environmentId)
        {
            try
            {
                _environmentsService.DeleteEnvironment(environmentId);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails {Status = 500, Title = $"{e.Message}"});
            }
        }
    }
}
