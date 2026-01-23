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
        var portStr = Environment.GetEnvironmentVariable("SMTP__PORT");
        var useSslStr = Environment.GetEnvironmentVariable("SMTP__USESSL");
        var timeoutStr = Environment.GetEnvironmentVariable("SMTP__TIMEOUTMS");

        return new SmtpSettings
        {
            Host = Environment.GetEnvironmentVariable("SMTP__HOST") ?? "smtp.gmail.com",
            Port = int.TryParse(portStr, out var port) ? port : 587,
            Username = Environment.GetEnvironmentVariable("SMTP__USERNAME"),
            Password = Environment.GetEnvironmentVariable("SMTP__PASSWORD"),
            UseSsl = bool.TryParse(useSslStr, out var useSsl) ? useSsl : true,
            FromName = Environment.GetEnvironmentVariable("SMTP__FROMNAME") ?? "MailSender",
            FromEmail = Environment.GetEnvironmentVariable("SMTP__FROMEMAIL"),
            TimeoutMs = int.TryParse(timeoutStr, out var timeout) ? timeout : 30000
        };
    }
}
