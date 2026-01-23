using MailSender.Core.Models;

namespace MailSender.Core.Services;

public interface IEmailService
{
    Task<EmailResponse> SendEmailAsync(EmailRequest emailRequest);
}
