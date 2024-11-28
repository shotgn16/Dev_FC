using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace ForestChurches.Components.Email
{
    public class MailRepository : IEmailSender, iEmail
    {
        public string _templatePath;
        private readonly IConfiguration _configuration;
        private readonly Configuration.Configuration _configurationClass;

        public MailRepository(IConfiguration configuration, Configuration.Configuration configurationClass)
        {
            _configuration = configuration;
            _configurationClass = configurationClass;
        }

        public async Task StartEmailAsync(string recipientEmail, Dictionary<string, string> userData, string subject, string templatePath)
        {
            _templatePath = templatePath;

            var templateContent = await ReadTemplateAsync();
            var populatedTemplate = await PopulateTemplate(templateContent, userData);
            await SendEmailAsync(recipientEmail, populatedTemplate, subject);;
        }

        private async Task<string> ReadTemplateAsync()
        {
            using (StreamReader reader = new StreamReader(_templatePath))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private Task<string> PopulateTemplate(string templateContent, Dictionary<string, string> userData)
        {

            foreach (var item in userData)
            {
                templateContent = templateContent.Replace(item.Key, item.Value);
            }
            return Task.FromResult(templateContent);
        }

        public async Task SendEmailAsync(string recipientEmail, string emailContent, string subject)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Forest Churches", "info@forestchurches.co.uk"));
            message.To.Add(new MailboxAddress("Recipient Name", recipientEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = emailContent };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync("smtp.ionos.co.uk", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("info@forestchurches.co.uk", _configurationClass.Client.GetSecret("smtp-auth-password").Value.Value);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
