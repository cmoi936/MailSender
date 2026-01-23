using MailSender.Core.Models;
using MailSender.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CoreEmailService = MailSender.Core.Services.SmtpEmailService;

namespace MailSender.Services
{
    public interface IEmailService
    {
        Task<EmailResponse> SendEmailAsync(EmailRequest emailRequest);
    }

    public class SmtpEmailService : IEmailService
    {
        private readonly CoreEmailService _coreEmailService;

        public SmtpEmailService(ILogger<SmtpEmailService> logger, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            // Build SmtpSettings from configuration
            var settings = new SmtpSettings
            {
                Host = configuration["Smtp:Host"] ?? "smtp.gmail.com",
                Port = int.Parse(configuration["Smtp:Port"] ?? "587"),
                Username = configuration["Smtp:Username"] ?? throw new ArgumentException("SMTP Username is required"),
                Password = configuration["Smtp:Password"] ?? throw new ArgumentException("SMTP Password is required"),
                UseSsl = bool.Parse(configuration["Smtp:UseSsl"] ?? "true"),
                FromName = configuration["Smtp:FromName"] ?? "MailSender API",
                FromEmail = configuration["Smtp:FromEmail"],
                TimeoutMs = int.Parse(configuration["Smtp:TimeoutMs"] ?? "30000")
            };

            // Create the core email service with a properly typed logger
            var coreLogger = loggerFactory.CreateLogger<CoreEmailService>();
            _coreEmailService = new CoreEmailService(settings, coreLogger);
        }

        public Task<EmailResponse> SendEmailAsync(EmailRequest emailRequest)
        {
            return _coreEmailService.SendEmailAsync(emailRequest);
        }
    }
}