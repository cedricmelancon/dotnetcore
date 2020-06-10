using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserApplication.Configurations;
using UserApplication.Data.Models;
using UserApplication.Services;

namespace UserApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly SqlConfiguration _dbConfiguration;

        public UserController(ILogger<UserController> logger, IUserService userService, SqlConfiguration configuration)
        {
            _logger = logger;
            _userService = userService;
            _dbConfiguration = configuration;

            _logger.LogDebug("The connection string found for endpoint: {endpoint}", _dbConfiguration.ConnectionString);
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserModel>> Get()
        {
            _logger.LogDebug("GET called.");

            IEnumerable<UserModel> users;

            try
            {
                users = _userService.GetUsers();

                if (!users.Any())
                {
                    _logger.LogWarning("No user found!");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while getting all users!");
                return Problem(e.Message, "UserController - GET", 501);
            }

            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserModel user)
        {
            _logger.LogDebug("POST Called.");

            try
            {
                await _userService.AddUserAsync(user);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while adding a user!");
                return Problem(e.Message, "UserController - POST", 501);
            }

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UserModel user)
        {
            _logger.LogDebug("PUT Called.");

            try
            {
                await _userService.UpdateUserAsync(user);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while updating a user!");
                return Problem(e.Message, "UserController - PUT", 501);
            }

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogDebug("DELETE Called.");

            try
            {
                await _userService.DeleteUserAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while adding a user!");
                return Problem(e.Message, "UserController - DELETE", 501);
            }

            return Ok();
        }
    }
}
