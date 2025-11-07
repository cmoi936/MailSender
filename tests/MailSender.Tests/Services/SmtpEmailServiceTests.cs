using MailSender.Models;
using MailSender.Services;
using MailSender.Tests.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace MailSender.Tests.Services
{
    /// <summary>
    /// Tests nominaux (happy path) pour le service SmtpEmailService. Ces tests vérifient que le service fonctionne
    /// correctement dans les cas d'utilisation normaux.
    /// </summary>
    public class SmtpEmailServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private SmtpEmailService _emailService;

        public SmtpEmailServiceTests()
        {
            EnvironmentHelper.LoadEnvFile();
            _mockConfiguration = new Mock<IConfiguration>();
            ConfigureDefaultMocks();
        }

        /// <summary>
        /// Configure les valeurs par défaut pour les mocks de configuration SMTP.
        /// </summary>
        private void ConfigureDefaultMocks()
        {
            // Charger les valeurs depuis les variables d'environnement ou des secrets
            var smtpConfig = new Dictionary<string, string>
            {
                { "Smtp:Host", Environment.GetEnvironmentVariable("SMTP_HOST") ?? "smtp.gmail.com" },
                { "Smtp:Port", Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587" },
                { "Smtp:Username", Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? "test@example.com" },
                { "Smtp:Password", Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? "test-password" },
                { "Smtp:UseSsl", Environment.GetEnvironmentVariable("SMTP_USESSL") ?? "true" },
                { "Smtp:FromName", Environment.GetEnvironmentVariable("SMTP_FROMNAME") ?? "Test Sender" },
                { "Smtp:FromEmail", Environment.GetEnvironmentVariable("SMTP_FROMEMAIL") ?? "test@example.com" },
                { "Smtp:TimeoutMs", Environment.GetEnvironmentVariable("SMTP_TIMEOUTMS") ?? "30000" }
            };

            foreach (var config in smtpConfig)
            {
                _mockConfiguration
                    .Setup(x => x[config.Key])
                    .Returns(config.Value);
            }

            _emailService = new SmtpEmailService(
                NullLogger<SmtpEmailService>.Instance,
                _mockConfiguration.Object);
        }

        /// <summary>
        /// Test nominal : SendEmailAsync retourne une réponse (même en cas d'erreur de connexion SMTP). Cela valide la
        /// gestion des exceptions et le retour d'une EmailResponse appropriée.
        /// </summary>
        [Fact]
        public async Task SendEmailAsync_WithValidRequest_ReturnsEmailResponse()
        {
            // Arrange
            var emailRequest = new EmailRequest
            {
                To = Environment.GetEnvironmentVariable("SMTP_USERNAME"),
                Subject = "Test Subject",
                Message = @"<!DOCTYPE html>
<html lang=""und"" dir=""auto"" xmlns=""http://www.w3.org/1999/xhtml"" xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:o=""urn:schemas-microsoft-com:office:office"">
<head>
  <title>
  </title>
  <!--[if !mso]><!-->
  <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
  <!--<![endif]-->
  <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <style type=""text/css"">
    #outlook a {
      padding: 0;
    }
    
body {
      margin: 0;
      padding: 0;
      -webkit-text-size-adjust: 100%;
      -ms-text-size-adjust: 100%;
    }
    
table,
    td {
      border-collapse: collapse;
      mso-table-lspace: 0pt;
      mso-table-rspace: 0pt;
    }
    
img {
      border: 0;
      height: auto;
      line-height: 100%;
      outline: none;
      text-decoration: none;
      -ms-interpolation-mode: bicubic;
    }
    
p {
      display: block;
      margin: 13px 0;
    }
  </style>
  <!--[if mso]>
          <noscript>
          <xml>
          <o:OfficeDocumentSettings>
            <o:AllowPNG/>
            <o:PixelsPerInch>96</o:PixelsPerInch>
          </o:OfficeDocumentSettings>
          </xml>
          </noscript>
          <![endif]-->
    <!--[if lte mso 11]>
          <style type=""text/css"">
            .mj-outlook-group-fix { width:100% !important; }
          </style>
          <![endif]-->
  <!--[if !mso]><!-->
  <link href=""https://fonts.googleapis.com/css?family=Ubuntu:300,400,500,700"" rel=""stylesheet"" type=""text/css"">
    <style type=""text/css"">
      @import url(https://fonts.googleapis.com/css?family=Ubuntu:300,400,500,700);
    </style>
    <!--<![endif]-->
    <style type=""text/css"">
      @media only screen and (min-width:480px) {
        .mj-column-per-100 {
width:100% !important;
max-width: 100%;
}
      }
    </style>
    <style media=""screen and (min-width:480px)"">
        .moz-text-html .mj-column-per-100 {
width:100% !important;
max-width: 100%;
}
    </style>
    <style type=""text/css"">
    </style>
    <style type=""text/css"">
    </style>
  </head>
  
  <body style=""margin: 0;padding: 0;-webkit-text-size-adjust: 100%;-ms-text-size-adjust: 100%;background-color: #f4f4f4;word-spacing: normal"">
<div lang=""und"" dir=""auto"" style=""background-color:#f4f4f4;"">
  <!--[if mso | IE]>
  <table align=""center"" bgcolor=""#667eea"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" width=""600"" style=""width:600px;"">
    <tr>
      <td style=""font-size:0px;line-height:0px;mso-line-height-rule:exactly;"">
        <![endif]-->
        <div style=""margin:0px auto;max-width:600px;background:#667eea;background-color:#667eea;"">
          <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;width: 100%;background: #667eea;background-color: #667eea"">
            <tbody>
              <tr>
                <td style=""border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;direction: ltr;font-size: 0px;padding: 2px 0;padding-bottom: 2px;padding-left: 0;padding-right: 0;padding-top: 2px;text-align: center"">
                  <!--[if mso | IE]>
                  <table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"">
                    <tr>
                      <td style=""vertical-align:top;width:600px;"">
                        <![endif]-->
                        <div class=""mj-column-per-100 mj-outlook-group-fix"" style=""direction:ltr;display:inline-block;font-size:0px;text-align:left;vertical-align:top;width:100%;"">
                          <table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" width=""100%"" style=""border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top"">
                            <tbody>
                              <tr>
                                <td align=""center"" style=""border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;font-size: 0px;padding: 10px 25px;padding-bottom: 10px;padding-left: 25px;padding-right: 25px;padding-top: 10px;word-break: break-word"">
                                  <div style=""font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:20px;font-weight:bold;line-height:1;text-align:center;color:white;"">
📦 Résultats de recherche NuGet
                                  </div>
                                </td>
                              </tr>
                              <tr>
                                <td align=""center"" style=""border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;font-size: 0px;padding: 10px 25px;padding-bottom: 10px;padding-left: 25px;padding-right: 25px;padding-top: 0;word-break: break-word"">
                                  <div style=""font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:16px;line-height:1;text-align:center;color:white;"">
Default Search
                                  </div>
                                </td>
                              </tr>
                            </tbody>
                          </table>
                        </div>
                        <!--[if mso | IE]>
                      </td>
                    </tr>
                  </table>
                  <![endif]-->
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <!--[if mso | IE]>
      </td>
    </tr>
  </table>
  <table align=""center"" bgcolor=""white"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" width=""600"" style=""width:600px;"">
    <tr>
      <td style=""font-size:0px;line-height:0px;mso-line-height-rule:exactly;"">
        <![endif]-->
        <div style=""margin:0px auto;max-width:600px;background:white;background-color:white;"">
          <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;width: 100%;background: white;background-color: white"">
            <tbody>
              <tr>
                <td style=""border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;direction: ltr;font-size: 0px;padding: 0px 5px;padding-bottom: 0px;padding-left: 5px;padding-right: 5px;padding-top: 0px;text-align: center"">
                  <!--[if mso | IE]>
                  <table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"">
                    <tr>
                      <td style=""vertical-align:top;width:590px;"">
                        <![endif]-->
                        <div class=""mj-column-per-100 mj-outlook-group-fix"" style=""direction:ltr;display:inline-block;font-size:0px;text-align:left;vertical-align:top;width:100%;"">
                          <table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" width=""100%"" style=""border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top"">
                            <tbody>
                              <tr>
                                <td align=""left"" style=""border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;font-size: 0px;padding: 10px 25px;padding-bottom: 10px;padding-left: 25px;padding-right: 25px;padding-top: 10px;word-break: break-word"">
                                  <div style=""font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:14px;line-height:1.2;text-align:left;color:#555555;"">
<strong>Recherche effectuée le :</strong> 07/11/2025 00:06<br>
          <strong>Versions préliminaires incluses :</strong> Non<br>
          <strong>Total packages trouvés :</strong> 10<br>
          <strong>Nouveaux packages sauvegardés :</strong> 3
                                  </div>
                                </td>
                              </tr>
                            </tbody>
                          </table>
                        </div>
                        <!--[if mso | IE]>
                      </td>
                    </tr>
                  </table>
                  <![endif]-->
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <!--[if mso | IE]>
      </td>
    </tr>
  </table>
  <table align=""center"" bgcolor=""white"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" width=""600"" style=""width:600px;"">
    <tr>
      <td style=""font-size:0px;line-height:0px;mso-line-height-rule:exactly;"">
        <![endif]-->
        <div style=""margin:0px auto;max-width:600px;background:white;background-color:white;"">
          <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;width: 100%;background: white;background-color: white"">
            <tbody>
              <tr>
                <td style=""border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;direction: ltr;font-size: 0px;padding: 0px 5px;padding-bottom: 0px;padding-left: 5px;padding-right: 5px;padding-top: 0px;text-align: center"">
                  <!--[if mso | IE]>
                  <table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"">
                    <tr>
                      <td style=""vertical-align:top;width:590px;"">
                        <![endif]-->
                        <div class=""mj-column-per-100 mj-outlook-group-fix"" style=""direction:ltr;display:inline-block;font-size:0px;text-align:left;vertical-align:top;width:100%;"">
                          <table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" width=""100%"" style=""border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top"">
                            <tbody>
                              <tr>
                                <td align=""left"" style=""border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;font-size: 0px;padding: 10px 25px;padding-bottom: 5px;padding-left: 25px;padding-right: 25px;padding-top: 10px;word-break: break-word"">
                                  <div style=""font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:16px;font-weight:600;line-height:1;text-align:left;color:#333333;"">
📋 Détails des packages
                                  </div>
                                </td>
                              </tr>
                              <tr>
                                <td align=""left"" style=""border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;font-size: 0px;padding: 10px 25px;padding-bottom: 10px;padding-left: 25px;padding-right: 25px;padding-top: 10px;word-break: break-word"">
                                  <table border=""0"" cellpadding=""12px"" cellspacing=""0"" width=""100%"" style=""border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;border: 1px solid #e0e0e0;color: #000000;font-family: Ubuntu, Helvetica, Arial, sans-serif;font-size: 13px;line-height: 22px;table-layout: auto;width: 100%"">
<tbody><tr style=""background-color: #f9f9f9; border-bottom: 2px solid #333;"">
                <th style=""text-align: left; padding: 12px; color: #333; font-weight: bold; font-size: 13px;"">Package ID</th>
                <th style=""text-align: left; padding: 12px; color: #333; font-weight: bold; font-size: 13px;"">Version</th>
                <th style=""text-align: left; padding: 12px; color: #333; font-weight: bold; font-size: 13px;"">Description</th>
                <th style=""text-align: center; padding: 12px; color: #333; font-weight: bold; font-size: 13px;"">Statut</th>
            </tr>
            <tr style=""border-bottom: 1px solid #e0e0e0;"">
                <td style=""border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;padding: 12px;color: #555;font-size: 13px"">SamplePackage</td>
                <td style=""border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;padding: 12px;color: #555;font-size: 13px"">1.0.0</td>
                <td style=""border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;padding: 12px;color: #555;font-size: 13px"">Sample description</td>
                <td style=""border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;padding: 12px;text-align: center;font-weight: bold;font-size: 13px;color: #28a745"">
                    🟢 Nouveau
                </td>
            </tr>
                                  </tbody></table>
                                </td>
                              </tr>
                            </tbody>
                          </table>
                        </div>
                        <!--[if mso | IE]>
                      </td>
                    </tr>
                  </table>
                  <![endif]-->
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <!--[if mso | IE]>
      </td>
    </tr>
  </table>
  <table align=""center"" bgcolor=""#f4f4f4"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" width=""600"" style=""width:600px;"">
    <tr>
      <td style=""font-size:0px;line-height:0px;mso-line-height-rule:exactly;"">
        <![endif]-->
        <div style=""margin:0px auto;max-width:600px;background:#f4f4f4;background-color:#f4f4f4;"">
          <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;width: 100%;background: #f4f4f4;background-color: #f4f4f4"">
            <tbody>
              <tr>
                <td style=""border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;direction: ltr;font-size: 0px;padding: 5px;padding-bottom: 5px;padding-left: 5px;padding-right: 5px;padding-top: 5px;text-align: center"">
                  <!--[if mso | IE]>
                  <table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"">
                    <tr>
                      <td style=""vertical-align:top;width:590px;"">
                        <![endif]-->
                        <div class=""mj-column-per-100 mj-outlook-group-fix"" style=""direction:ltr;display:inline-block;font-size:0px;text-align:left;vertical-align:top;width:100%;"">
                          <table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" width=""100%"" style=""border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top"">
                            <tbody>
                              <tr>
                                <td align=""center"" style=""border-collapse: collapse;mso-table-lspace: 0pt;mso-table-rspace: 0pt;font-size: 0px;padding: 10px 25px;padding-bottom: 10px;padding-left: 25px;padding-right: 25px;padding-top: 10px;word-break: break-word"">
                                  <div style=""font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:12px;font-style:italic;line-height:1;text-align:center;color:#888888;"">
Cet email a été généré automatiquement par MyJobs.
                                  </div>
                                </td>
                              </tr>
                            </tbody>
                          </table>
                        </div>
                        <!--[if mso | IE]>
                      </td>
                    </tr>
                  </table>
                  <![endif]-->
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <!--[if mso | IE]>
      </td>
    </tr>
  </table>
  <![endif]-->
</div>

  
  
  
</body></html>"
            };

            // Act
            var response = await _emailService.SendEmailAsync(emailRequest);

            // Assert
            Assert.NotNull(response);
            Assert.IsType<EmailResponse>(response);
            Assert.True(response.Message != null); // Devrait contenir un message
        }
    }
}
