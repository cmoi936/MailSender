namespace MailSender.Tests.Helpers;

public static class EnvironmentHelper
{
    public static void LoadEnvFile()
    {
        var envFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            ".env.local"
        );

        if (File.Exists(envFilePath))
        {
            foreach (var line in File.ReadAllLines(envFilePath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                var parts = line.Split('=', 2);
                if (parts.Length == 2)
                {
                    Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
                }
            }
        }
    }
}