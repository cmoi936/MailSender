namespace MailSender.Core.Models;

public class SmtpSettings
{
    public string Host { get; set; } = "smtp.gmail.com";
    public int Port { get; set; } = 587;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public bool UseSsl { get; set; } = true;
    public string FromName { get; set; } = "MailSender";
    public string? FromEmail { get; set; }
    public int TimeoutMs { get; set; } = 30000;

    /// <summary>
    /// Creates SmtpSettings from environment variables
    /// </summary>
    public static SmtpSettings FromEnvironment()
    {
        return new SmtpSettings
        {
            Host = Environment.GetEnvironmentVariable("SMTP__HOST") ?? "smtp.gmail.com",
            Port = int.Parse(Environment.GetEnvironmentVariable("SMTP__PORT") ?? "587"),
            Username = Environment.GetEnvironmentVariable("SMTP__USERNAME"),
            Password = Environment.GetEnvironmentVariable("SMTP__PASSWORD"),
            UseSsl = bool.Parse(Environment.GetEnvironmentVariable("SMTP__USESSL") ?? "true"),
            FromName = Environment.GetEnvironmentVariable("SMTP__FROMNAME") ?? "MailSender",
            FromEmail = Environment.GetEnvironmentVariable("SMTP__FROMEMAIL"),
            TimeoutMs = int.Parse(Environment.GetEnvironmentVariable("SMTP__TIMEOUTMS") ?? "30000")
        };
    }
}
