using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace GSMAO.Server.Services
{
    public class EmailSender(IOptions<EmailSenderOptions> optionsAccessor, ILogger<EmailSender> logger) : IMyEmailSender
    {
        public EmailSenderOptions Options { get; } = optionsAccessor.Value;

        public async Task SendEmailAsync(string? toEmail, string subject, string htmlMessage, string addressee, string? textMessage = null)
        {
            try
            {
                string htmlTemplatePath = @"./EmailTemplate.html";
                string htmlTemplate = LoadHtmlTemplate(htmlTemplatePath);
                string html = htmlTemplate.Replace("{Subject}", subject).Replace("{BodyContent}", htmlMessage).Replace("{Addressee}", addressee);

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(this.Options.EmailFromAddress, this.Options.EmailFromName);
                mailMessage.To.Add(toEmail!);
                mailMessage.Body = textMessage;
                mailMessage.BodyEncoding = Encoding.UTF8;
                mailMessage.Subject = subject;
                mailMessage.SubjectEncoding = Encoding.UTF8;

                if (!string.IsNullOrEmpty(html))
                {
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, "text/html");
                    mailMessage.AlternateViews.Add(htmlView);
                }

                using (SmtpClient client = new SmtpClient(this.Options.Host, this.Options.Port))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(this.Options.Username, this.Options.Password);
                    client.EnableSsl = this.Options.EnableSSL;
                    await client.SendMailAsync(mailMessage);
                }
                logger.LogInformation($"Email enviado: {toEmail}, {subject}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error al enviar email: {ex.Message}");
                throw;
            }
        }

        private string LoadHtmlTemplate(string filePath)
        {
            return File.ReadAllText(filePath, Encoding.UTF8);
        }
    }
}
