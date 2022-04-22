﻿using System;
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
    public class JobsController : ControllerBase
    {
        private readonly IJobsService _jobsService;
        private readonly ILogger<JobsController> _logger;

        public JobsController(IJobsService jobsService, ILogger<JobsController> logger)
        {
            _jobsService = jobsService ?? throw new ArgumentNullException(nameof(jobsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// create job
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        [HttpPost("")]
        [ProducesResponseType(typeof(Job), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult CreateJob(Job job)
        {
            if (!ModelState.IsValid)
            {
                var serializableModelState = new SerializableError(ModelState);
                var modelStateJson = JsonConvert.SerializeObject(serializableModelState);
                _logger.LogError($"Invalid model for create job", modelStateJson);
                return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid model for create job" });
            }

            try
            {
                var createdJob = _jobsService.CreateJob(job);
                return Created($"/job/{createdJob.JobId.ToString()}", createdJob);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = e.Message, Detail = e.ToString() });
            }
        }

        /// <summary>
        /// get list of all jobs
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(List<Job>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetJobs()
        {
            try
            {
                return Ok(_jobsService.GetJobs());
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
            }
        }

        /// <summary>
        /// get job with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Job), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetJob(int id)
        {
            try
            {
                var job = _jobsService.GetJob(id);
                if (job != null)
                {
                    return Ok(job);
                }
                return NotFound(new ProblemDetails { Status = 404, Title = "Job not found" });
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
            }
        }

        /// <summary>
        /// update job with id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="job"></param>
        /// <returns></returns>
        [HttpPatch("{id}", Name = "UpdateJobPartial")]
        [HttpPut("{id}", Name = "UpdateJob")]
        [ProducesResponseType(typeof(Job), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult UpdateJob(int id, Job job)
        {
            if (!ModelState.IsValid)
            {
                var serializableModelState = new SerializableError(ModelState);
                var modelStateJson = JsonConvert.SerializeObject(serializableModelState);
                _logger.LogError($"Invalid model for update job {modelStateJson}");
                return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid model for update job" });
            }

            try
            {
                return Ok(_jobsService.UpdateJob(id, job));
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
            }
        }

        /// <summary>
        /// delete job with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult DeleteJob(int id)
        {
            try
            {
                _jobsService.DeleteJob(id);
                // In .NET Core 3.0 the OK / 200 object result returns no content instead of empty JSON.
                // In HTTP a OK / 200 response always needs a payload.
                // Invoking it with an empty object as a parameter will return an empty JSON object as the payload.
                return Ok(new object());
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
            }
        }
    }
}
