using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieAPI.ViewModels;
using System.Linq;

namespace MovieAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly ILogger<MovieController> _logger;

        public MovieController(ILogger<MovieController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public ActionResult Get([FromBody] MovieInfoViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = from error in ModelState
                             where error.Value.Errors.Any()
                             from err in error.Value.Errors
                             select string.IsNullOrWhiteSpace(err.ErrorMessage) ? err.Exception.Message : err.ErrorMessage;

                foreach (var error in errors)
                {
                    _logger.LogError(error);
                }

                return ValidationProblem();
            }

            return Ok();
        }
    }
}
