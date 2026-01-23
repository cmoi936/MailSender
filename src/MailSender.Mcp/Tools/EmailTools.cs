using System.ComponentModel;
using MailSender.Core.Models;
using MailSender.Core.Services;
using ModelContextProtocol.Server;

namespace MailSender.Mcp.Tools;

[McpServerToolType]
public class EmailTools
{
    [McpServerTool]
    [Description("Send an email via SMTP. Requires SMTP configuration via environment variables.")]
    public static async Task<string> SendEmail(
        [Description("Recipient email address")] string to,
        [Description("Email subject")] string subject,
        [Description("Email message body (can be HTML)")] string message,
        [Description("CC recipients, separated by semicolons (optional)")] string? cc = null,
        [Description("BCC recipients, separated by semicolons (optional)")] string? bcc = null)
    {
        try
        {
            // Get SMTP settings from environment variables
            var settings = SmtpSettings.FromEnvironment();
            
            // Create the email service with the settings
            var emailService = new SmtpEmailService(settings);

            // Create the email request
            var emailRequest = new EmailRequest
            {
                To = to,
                Subject = subject,
                Message = message,
                Cc = cc,
                Bcc = bcc
            };

            // Send the email
            var result = await emailService.SendEmailAsync(emailRequest);

            if (result.Success)
            {
                return $"Email sent successfully to {to}. Message ID: {result.MessageId}";
            }
            else
            {
                return $"Failed to send email: {result.Message}";
            }
        }
        catch (Exception ex)
        {
            return $"Failed to send email: {ex.Message}";
        }
    }
}
