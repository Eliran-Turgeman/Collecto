
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace EmailCollector.Api.Services.EmailSender;

public class EmailSender : IAppEmailSender, IEmailSender
{
    private readonly EmailConfiguration _emailConfig;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(EmailConfiguration emailConfig, ILogger<EmailSender> logger)
    {
        _emailConfig = emailConfig;
        _logger = logger;
    }

    public void SendEmail(Message message)
    {
        var emailMessage = CreateEmailMessage(message);
        Send(emailMessage);
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var msg = CreateEmailMessage(new Message(new string[] { email }, subject, htmlMessage));
        Send(msg);
        return Task.CompletedTask;
    }

    private MimeMessage CreateEmailMessage(Message message)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("Collecto", _emailConfig.From));
        emailMessage.To.AddRange(message.To);
        emailMessage.Subject = message.Subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Content };
        return emailMessage;
    }

    private void Send(MimeMessage mailMessage)
    {
        using (var client = new SmtpClient())
        {
            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                _logger.LogInformation($"Email creds = email:{_emailConfig.UserName}; pass:{_emailConfig.Password}");
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                client.Send(mailMessage);
            }
            catch
            {
                //log an error message or throw an exception or both.
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}
