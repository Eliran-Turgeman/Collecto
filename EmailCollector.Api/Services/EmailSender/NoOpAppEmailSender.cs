namespace EmailCollector.Api.Services.EmailSender;

public class NoOpAppEmailSender : IAppEmailSender
{
    public void SendEmail(Message message)
    {
        return;
    }
}
