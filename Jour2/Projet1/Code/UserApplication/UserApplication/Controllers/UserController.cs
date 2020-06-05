using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserApplication.Configurations;
using UserApplication.Models;
using UserApplication.Services;

namespace UserApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly CosmosDbConfiguration _dbConfiguration;

        public UserController(ILogger<UserController> logger, IUserService userService, CosmosDbConfiguration configuration)
        {
            _logger = logger;
            _userService = userService;
            _dbConfiguration = configuration;

            _logger.LogDebug("The connection string found: {cs}", _dbConfiguration.ConnectionString);
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserModel>> Get()
        {
            _logger.LogDebug("ControllerBase - GET called.");

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
    }
}
