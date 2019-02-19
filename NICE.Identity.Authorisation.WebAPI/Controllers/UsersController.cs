using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NICE.Identity.Authorisation.WebAPI.ApiModels.Responses;
using NICE.Identity.Authorisation.WebAPI.Services;
using User = NICE.Identity.Authorisation.WebAPI.ApiModels.Requests.User;

namespace NICE.Identity.Authorisation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
        }

        // PUT api/users
        [HttpPut]
        public async Task<ActionResult> Put([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Request failed validation");
            }

            try
            {
                await _usersService.CreateUser(user);

                return Ok();
            }
            catch (Exception e)
            {
                var error = new ErrorDetail()
                {
                    ErrorMessage = e.Message
                };

                return StatusCode(503, error);
            }
        }
    }
}