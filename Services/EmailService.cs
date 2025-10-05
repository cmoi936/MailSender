using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MailSender.Models;

namespace MailSender.Services
{
    public interface IEmailService
    {
        Task<EmailResponse> SendEmailAsync(EmailRequest emailRequest);
    }

    public class SmtpEmailService : IEmailService
    {
        private readonly ILogger<SmtpEmailService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _host;
        private readonly int _port;
        private readonly string _user;
        private readonly string _password;
        private readonly bool _useSsl;
        private readonly string _fromName;
        private readonly string _fromEmail;

        public SmtpEmailService(ILogger<SmtpEmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            
            // Configuration SMTP pour Gmail
            _host = _configuration["Smtp:Host"] ?? "smtp.gmail.com";
            _port = int.Parse(_configuration["Smtp:Port"] ?? "587");
            _user = _configuration["Smtp:Username"] ?? throw new ArgumentException("SMTP Username is required");
            _password = _configuration["Smtp:Password"] ?? throw new ArgumentException("SMTP Password is required");
            _useSsl = bool.Parse(_configuration["Smtp:UseSsl"] ?? "true");
            _fromName = _configuration["Smtp:FromName"] ?? "MailSender API";
            _fromEmail = _configuration["Smtp:FromEmail"] ?? _user;
        }

        public async Task<EmailResponse> SendEmailAsync(EmailRequest emailRequest)
        {
            try
            {
                var message = CreateMessage(emailRequest);
                
                using var client = new SmtpClient();
                
                // Configuration du timeout si spécifié
                var timeoutMs = int.Parse(_configuration["Smtp:TimeoutMs"] ?? "30000");
                client.Timeout = timeoutMs;

                // Connexion au serveur SMTP
                if (_useSsl)
                {
                    await client.ConnectAsync(_host, _port, SecureSocketOptions.StartTls);
                }
                else
                {
                    await client.ConnectAsync(_host, _port, SecureSocketOptions.None);
                }

                // Authentification
                if (!string.IsNullOrEmpty(_user))
                {
                    await client.AuthenticateAsync(_user, _password);
                }

                // Envoi du message
                var result = await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {To} with subject: {Subject}", 
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
                _logger.LogError(ex, "Failed to send email to {To}", emailRequest.To);
                return new EmailResponse
                {
                    Success = false,
                    Message = $"Failed to send email: {ex.Message}"
                };
            }
        }

        private MimeMessage CreateMessage(EmailRequest emailRequest)
        {
            var message = new MimeMessage();
            
            // Expéditeur
            message.From.Add(new MailboxAddress(_fromName, _fromEmail));
            
            // Destinataire principal
            message.To.Add(MailboxAddress.Parse(emailRequest.To));
            
            // Copie (CC)
            if (!string.IsNullOrEmpty(emailRequest.Cc))
            {
                var ccAddresses = emailRequest.Cc.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (var cc in ccAddresses)
                {
                    message.Cc.Add(MailboxAddress.Parse(cc.Trim()));
                }
            }
            
            // Copie cachée (BCC)
            if (!string.IsNullOrEmpty(emailRequest.Bcc))
            {
                var bccAddresses = emailRequest.Bcc.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (var bcc in bccAddresses)
                {
                    message.Bcc.Add(MailboxAddress.Parse(bcc.Trim()));
                }
            }
            
            // Sujet
            message.Subject = emailRequest.Subject;
            
            // Corps du message
            var bodyBuilder = new BodyBuilder
            {
                TextBody = emailRequest.Message,
                HtmlBody = $"<p>{emailRequest.Message.Replace("\n", "<br>")}</p>"
            };
            
            message.Body = bodyBuilder.ToMessageBody();
            
            return message;
        }
    }
}