using EmailCollector.Domain.Entities;

namespace EmailCollector.Api.Services.EmailSender;

public interface IAppEmailSender
{
    void SendEmail(Message message, SmtpEmailSettings? formEmailSettings);
}
