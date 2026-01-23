using MailKit.Net.Smtp;
using MailKit.Security;
using MailSender.Core.Models;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace MailSender.Core.Services;

public class SmtpEmailService : IEmailService
{
    private readonly ILogger<SmtpEmailService>? _logger;
    private readonly SmtpSettings _settings;

    public SmtpEmailService(SmtpSettings settings, ILogger<SmtpEmailService>? logger = null)
    {
        _settings = settings;
        _logger = logger;
    }

    public async Task<EmailResponse> SendEmailAsync(EmailRequest emailRequest)
    {
        try
        {
            ValidateSettings();

            var message = CreateMessage(emailRequest);

            using var client = new SmtpClient();
            client.Timeout = _settings.TimeoutMs;

            // Connect to SMTP server
            if (_settings.UseSsl)
            {
                await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            }
            else
            {
                await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.None);
            }

            // Authenticate
            if (!string.IsNullOrEmpty(_settings.Username))
            {
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
            }

            // Send the message
            var result = await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger?.LogInformation("Email sent successfully to {To} with subject: {Subject}",
                emailRequest.To, emailRequest.Subject);

            return new EmailResponse
            {
                Success = true,
                Message = "Email sent successfully",
                MessageId = result
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to send email to {To}", emailRequest.To);
            return new EmailResponse
            {
                Success = false,
                Message = $"Failed to send email: {ex.Message}"
            };
        }
    }

    private void ValidateSettings()
    {
        if (string.IsNullOrEmpty(_settings.Username))
            throw new InvalidOperationException("SMTP Username is required");
        if (string.IsNullOrEmpty(_settings.Password))
            throw new InvalidOperationException("SMTP Password is required");
    }

    private MimeMessage CreateMessage(EmailRequest emailRequest)
    {
        var message = new MimeMessage();

        // Sender
        var fromEmail = _settings.FromEmail ?? _settings.Username!;
        message.From.Add(new MailboxAddress(_settings.FromName, fromEmail));

        // Main recipient
        message.To.Add(MailboxAddress.Parse(emailRequest.To));

        // CC recipients
        if (!string.IsNullOrEmpty(emailRequest.Cc))
        {
            var ccAddresses = emailRequest.Cc.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var cc in ccAddresses)
            {
                message.Cc.Add(MailboxAddress.Parse(cc.Trim()));
            }
        }

        // BCC recipients
        if (!string.IsNullOrEmpty(emailRequest.Bcc))
        {
            var bccAddresses = emailRequest.Bcc.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var bcc in bccAddresses)
            {
                message.Bcc.Add(MailboxAddress.Parse(bcc.Trim()));
            }
        }

        // Subject
        message.Subject = emailRequest.Subject;

        // Body
        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = emailRequest.Message
        };
        message.Body = bodyBuilder.ToMessageBody();

        return message;
    }
}
