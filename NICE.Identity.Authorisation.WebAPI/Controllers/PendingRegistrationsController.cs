﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authorisation.WebAPI.Services;
using System;
using System.Threading.Tasks;

namespace NICE.Identity.Authorisation.WebAPI.Controllers
{
	[Route("api/[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration)]
	[ApiController]
    public class PendingRegistrationsController : ControllerBase
    {
        private readonly ILogger<PendingRegistrationsController> _logger;
        private readonly IUsersService _usersService;
        private const int daysToKeepPendingRegistration = 30;

        public PendingRegistrationsController(ILogger<PendingRegistrationsController> logger, IUsersService usersService)
        {
	        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
	        _usersService = usersService;
        }

        /// <summary>
		/// This endpoint is intended to be hit once per day at 8am, by a scheduled event.
		/// </summary>
		/// <returns></returns>
		[HttpDelete("DeleteAllOverAgeWithNotification")]
		[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Produces("application/json")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration)]
		public async Task<IActionResult> DeleteAllOverAgeWithNotification()
		{
			try
			{
				await _usersService.DeleteRegistrationsOlderThan(notify: true, daysToKeepPendingRegistration);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
			}
		}

		/// <summary>
		/// This endpoint should only be hit manually once, to clear out the old notifications.
		/// </summary>
		/// <returns></returns>
		[HttpDelete("DeleteAllOverAgeWithoutNotification")]
		[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Produces("application/json")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration)]
		public async Task<IActionResult> DeleteAllOverAgeWithoutNotification()
		{
			try
			{
				await _usersService.DeleteRegistrationsOlderThan(notify: false, daysToKeepPendingRegistration);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
			}
		}
	}
}