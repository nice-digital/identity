using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.Services;

namespace NICE.Identity.Authorisation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderManagementController : ControllerBase
    {
        private readonly ILogger<ProviderManagementController> _logger;
        private readonly IProviderManagementService _providerManagementService;

        public ProviderManagementController(IProviderManagementService providerManagementService, ILogger<ProviderManagementController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _providerManagementService = providerManagementService ?? throw new ArgumentNullException(nameof(providerManagementService));
        }

        /// <summary>
        /// Revokes all the refresh tokens for given user
        /// </summary>
        /// <param name="nameIdentifier"></param>
        /// <returns></returns>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public async Task<IActionResult> RevokeRefreshTokensForUserAsync([FromBody] string nameIdentifier)
        {
            try
            {
                await _providerManagementService.RevokeRefreshTokensForUser(nameIdentifier);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = $"{e.Message}" });
            }
        }
    }
}
