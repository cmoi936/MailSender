using Microsoft.AspNetCore.Mvc;

namespace MailSender.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        /// <returns>Status de l'application</returns>
        [HttpGet]
        public ActionResult<object> GetHealth()
        {
            _logger.LogInformation("Health check requested");
            
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                service = "MailSender API",
                version = "1.0.0"
            });
        }
    }
}