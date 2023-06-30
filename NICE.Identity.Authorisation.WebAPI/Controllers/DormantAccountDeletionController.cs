using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    public class DormantAccountDeletionController : ControllerBase
    {
        private readonly ILogger<DormantAccountDeletionController> _logger;
        private readonly IUsersService _usersService;

        public DormantAccountDeletionController(ILogger<DormantAccountDeletionController> logger, IUsersService usersService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _usersService = usersService;
        }

        /// <summary>
		/// Delete dormant accounts
		/// </summary>
		/// <returns></returns>
		[HttpDelete("DeleteDormantAccounts")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.API.UserAdministration)]
        public async Task<IActionResult> DeleteDormantAccounts()
        {
            try
            {
                await _usersService.SendPendingDeletionEmails(DateTime.Now);
                await _usersService.DeleteDormantAccounts(DateTime.Now);

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
            }
        }
    }
}