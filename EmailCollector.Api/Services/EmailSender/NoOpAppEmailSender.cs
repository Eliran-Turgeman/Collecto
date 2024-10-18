using EmailCollector.Domain.Entities;

namespace EmailCollector.Api.Services.EmailSender;

public class NoOpAppEmailSender : IAppEmailSender
{
    public void SendEmail(Message message, SmtpEmailSettings? smtpEmailSettings)
    {
        return;
    }
}
