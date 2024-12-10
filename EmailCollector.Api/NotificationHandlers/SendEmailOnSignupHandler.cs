using EmailCollector.Api.Repositories;
using EmailCollector.Api.Services.EmailSender;
using EmailCollector.Domain.Notifications;
using MediatR;

namespace EmailCollector.Api.NotificationHandlers;

public class SendEmailOnSignupHandler : INotificationHandler<EmailAddedSuccessfullyNotification>
{
    private readonly IAppEmailSender _emailSender;
    private readonly ISignupFormRepository _signupFormRepository;
    private readonly ISmtpEmailSettingsRepository _smtpEmailSettingsRepository;
    private readonly ILogger<SendEmailOnSignupHandler> _logger;

    public SendEmailOnSignupHandler(IAppEmailSender emailSender,
        ILogger<SendEmailOnSignupHandler> logger,
        ISignupFormRepository signupFormRepository,
        ISmtpEmailSettingsRepository smtpEmailSettingsRepository)
    {
        _emailSender = emailSender;
        _logger = logger;
        _signupFormRepository = signupFormRepository;
        _smtpEmailSettingsRepository = smtpEmailSettingsRepository;
    }

    public async Task Handle(EmailAddedSuccessfullyNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Sending welcome email to {notification.EmailAddress}.");
        var form = await _signupFormRepository.GetByIdAsync(notification.SignupFormId);
        var message = new Message(new string[] { notification.EmailAddress }, $"You signup was registered to {form.FormName}", "notification working!");
        var smtpConfiguration = await _smtpEmailSettingsRepository.GetByIdAsync(notification.SignupFormId);
        _emailSender.SendEmail(message, smtpConfiguration);

        _logger.LogInformation($"Welcome email sent to {notification.EmailAddress}.");
    }
}
