namespace GSMAO.Server.Services
{
    public interface IMyEmailSender
    {
        Task SendEmailAsync(string? toEmail,
                            string subject,
                            string htmlMessage,
                            string addressee,
                            string? textMessage = null);
    }
}
