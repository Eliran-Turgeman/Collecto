
using EmailCollector.Domain.Entities;
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

    public void SendEmail(Message message, SmtpEmailSettings? formEmailSettings)
    {
        var emailMessage = CreateEmailMessage(message, formEmailSettings);
        SendSmtp(emailMessage, formEmailSettings);
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var msg = CreateEmailMessage(new Message(new string[] { email }, subject, htmlMessage), default);
        SendSmtp(msg, default);
        return Task.CompletedTask;
    }

    private MimeMessage CreateEmailMessage(Message message, SmtpEmailSettings? formEmailSettings)
    {
        var emailMessage = new MimeMessage();
        emailMessage.To.AddRange(message.To);
        emailMessage.Subject = message.Subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Content };

        if (formEmailSettings != null)
        {
            emailMessage.From.Add(new MailboxAddress("Collecto", formEmailSettings.EmailFrom));

        }
        else
        {
            emailMessage.From.Add(new MailboxAddress("Collecto", _emailConfig.From));

        }

        return emailMessage;
    }

    private void SendSmtp(MimeMessage mailMessage, SmtpEmailSettings? formEmailSettings)
    {
        using (var client = new SmtpClient())
        {
            try
            {
                var smtpServer = formEmailSettings != null ? formEmailSettings.SmtpServer : _emailConfig.SmtpServer;
                var port = formEmailSettings != null ? formEmailSettings.SmtpPort : _emailConfig.Port;
                var userName = formEmailSettings != null ? formEmailSettings.SmtpUsername : _emailConfig.UserName;
                var password = formEmailSettings != null ? formEmailSettings.SmtpPassword : _emailConfig.Password;

                client.Connect(smtpServer, port, true);
                client.Authenticate(userName, password);
                client.Send(mailMessage);
            }
            catch
            {
                _logger.LogError("Failed to send email.");
            }
            finally
            {
                client.Disconnect(true);
            }
        }
    }
}
