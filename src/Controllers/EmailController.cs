using Microsoft.AspNetCore.Mvc;
using MailSender.Models;
using MailSender.Services;

namespace MailSender.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(IEmailService emailService, ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Envoie un email via SMTP
        /// </summary>
        /// <param name="emailRequest">Les détails de l'email à envoyer</param>
        /// <returns>Le résultat de l'envoi</returns>
        [HttpPost("send")]
        public async Task<ActionResult<EmailResponse>> SendEmail([FromBody] EmailRequest emailRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Attempting to send email to {To} with subject: {Subject}", 
                emailRequest.To, emailRequest.Subject);

            var result = await _emailService.SendEmailAsync(emailRequest);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return StatusCode(500, result);
            }
        }
    }
}