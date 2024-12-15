using EmailCollector.Api.Repositories;
using EmailCollector.Api.Services.CustomEmailTemplates;
using EmailCollector.Api.Services.EmailSender;
using EmailCollector.Domain.Entities;
using EmailCollector.Domain.Enums;
using EmailCollector.Domain.Notifications;
using MediatR;

namespace EmailCollector.Api.NotificationHandlers;

public class SendEmailOnSignupHandler : INotificationHandler<EmailAddedSuccessfullyNotification>
{
    private readonly IAppEmailSender _emailSender;
    private readonly ISignupFormRepository _signupFormRepository;
    private readonly IRepository<SmtpEmailSettings> _smtpEmailSettingsRepository;
    private readonly ICustomEmailTemplatesService _customEmailTemplatesService;
    private readonly ILogger<SendEmailOnSignupHandler> _logger;

    public SendEmailOnSignupHandler(IAppEmailSender emailSender,
        ILogger<SendEmailOnSignupHandler> logger,
        ISignupFormRepository signupFormRepository,
        IRepository<SmtpEmailSettings> smtpEmailSettingsRepository,
        ICustomEmailTemplatesService customEmailTemplatesService)
    {
        _emailSender = emailSender;
        _logger = logger;
        _signupFormRepository = signupFormRepository;
        _smtpEmailSettingsRepository = smtpEmailSettingsRepository;
        _customEmailTemplatesService = customEmailTemplatesService;
    }

    public async Task Handle(EmailAddedSuccessfullyNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Sending welcome email to {notification.EmailAddress}.");
        var form = await _signupFormRepository.GetByIdAsync(notification.SignupFormId);
        var customTemplate = await _customEmailTemplatesService.GetCustomEmailTemplateByFormIdAndEvent(form.Id, TemplateEvent.EmailConfirmed);

        if (customTemplate == null)
        {
            _logger.LogInformation($"No custom welcome email template exists for form {form.Id}");
            return;
        }
        
        var message = new Message([notification.EmailAddress], customTemplate.TemplateSubject, customTemplate.TemplateBody);
        var smtpConfiguration = await _smtpEmailSettingsRepository.GetByIdAsync(notification.SignupFormId);
        
        _emailSender.SendEmail(message, smtpConfiguration);

        _logger.LogInformation($"Welcome email sent to {notification.EmailAddress}.");
    }
}
