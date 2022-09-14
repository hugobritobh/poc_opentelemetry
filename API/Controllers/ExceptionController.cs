using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExceptionController : ControllerBase
    {
        private readonly ILogger<ExceptionController> _logger;

        public ExceptionController(ILogger<ExceptionController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "ProcessException")]
        public ActionResult Get()
        {
            var sum = 0;

            for (int i = 0; i < 9999999; i++)
            {
                sum = Random.Shared.Next(-20, 55) + Random.Shared.Next(-20, 55) + Random.Shared.Next(-20, 55) + Random.Shared.Next(-20, 55);
            }

            if (sum > 0)
            {
                throw new ArgumentException("Test API");
            }

            return Ok();
        }
    }
}