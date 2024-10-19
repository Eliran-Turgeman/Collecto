using EmailCollector.Api.Configurations;
using EmailCollector.Api.DTOs;
using EmailCollector.Api.Repositories;
using EmailCollector.Api.Services.EmailSender;
using EmailCollector.Domain.Entities;
using EmailCollector.Domain.Enums;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.RegularExpressions;

namespace EmailCollector.Api.Services;

public class EmailSignupService : IEmailSignupService
{
    private readonly IEmailSignupRepository _emailSignupRepository;
    private readonly ISignupFormRepository _signupFormRepository;
    private readonly ILogger<EmailSignupService> _logger;
    private readonly IDnsLookupService _dnsLookupService;
    private readonly IDistributedCache _signupCandidatesCache;
    private readonly IAppEmailSender _emailSender;
    private readonly IFeatureToggles _featureTogglesService;
    private readonly ISmtpEmailSettingsRepository _smtpEmailSettingsRepository;

    private static readonly Regex EmailRegex = new Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public EmailSignupService(IEmailSignupRepository emailSignupRepository,
        ISignupFormRepository signupFormRepository,
        ILogger<EmailSignupService> logger,
        IDnsLookupService dnsLookupService,
        IDistributedCache signupCandidatesCache,
        IAppEmailSender emailSender,
        IFeatureToggles featureTogglesService,
        ISmtpEmailSettingsRepository smtpEmailSettingsRepository)
    {
        _emailSignupRepository = emailSignupRepository;
        _signupFormRepository = signupFormRepository;
        _logger = logger;
        _dnsLookupService = dnsLookupService;
        _signupCandidatesCache = signupCandidatesCache;
        _emailSender = emailSender;
        _featureTogglesService = featureTogglesService;
        _smtpEmailSettingsRepository = smtpEmailSettingsRepository;
    }

    public async Task<IEnumerable<EmailSignupDto>?> GetSignupsByFormIdAsync(int formId, Guid userId)
    {
        var form = await _signupFormRepository.GetByIdAsync(formId);

        if (form == null || form.CreatedBy != userId)
        {
            _logger.LogInformation("Form not found or user is not the creator.");
            return null;
        }

        var signups = await _emailSignupRepository.GetByFormIdAsync(formId);
        _logger.LogInformation($"Found {signups.Count()} signups for form {formId}.");

        return signups.Select(signup => new EmailSignupDto
        {
            Email = signup.EmailAddress,
            FormId = signup.SignupFormId,
            SignupDate = signup.SignupDate,
        });
    }

    public async Task<SignupResultDto> SubmitEmailAsync(EmailSignupDto emailSignupDto)
    {
        _logger.LogInformation($"Submitting email address {emailSignupDto.Email} for form {emailSignupDto.FormId}.");

        if (!ValidateEmail(emailSignupDto.Email))
        {
            return await Task.FromResult(new SignupResultDto
            {
                Success = false,
                Message = "Invalid email address.",
                ErrorCode = EmailSignupErrorCode.InvalidEmail,
            });
        }

        var form = await _signupFormRepository.GetByIdAsync(emailSignupDto.FormId);
        if (form == null)
        {
            _logger.LogInformation("Form not found.");
            return await Task.FromResult(new SignupResultDto
            {
                Success = false,
                Message = "Form not found.",
                ErrorCode = EmailSignupErrorCode.FormNotFound,
            });
        }

        var allExistingSignups = await _emailSignupRepository.GetByFormIdAsync(form.Id);
        if (allExistingSignups.Any(s => s.EmailAddress == emailSignupDto.Email))
        {
            _logger.LogInformation("Email address already signed up.");
            return await Task.FromResult(new SignupResultDto
            {
                Success = false,
                Message = "Email address already signed up.",
                ErrorCode = EmailSignupErrorCode.EmailAlreadySignedUp,
            });
        }

        if (form.Status != FormStatus.Active)
        {
            _logger.LogInformation("Form is not active.");
            return await Task.FromResult(new SignupResultDto
            {
                Success = false,
                Message = "Form is not active.",
                ErrorCode = EmailSignupErrorCode.FormNotActive,
            });
        }

        var emailSignup = new EmailSignup
        {
            EmailAddress = emailSignupDto.Email,
            SignupFormId = emailSignupDto.FormId,
        };

        if (!_featureTogglesService.IsEmailConfirmationEnabled())
        {
            _logger.LogInformation("Email confirmation is disabled, signing up email address.");
            await _emailSignupRepository.AddAsync(emailSignup);
            return await Task.FromResult(new SignupResultDto
            {
                Success = true,
                Message = "Email address signed up successfully.",
            });
        }

        _logger.LogInformation("Email confirmation is enabled, sending confirmation email.");

        var signupCandidateValue = $"formId:{emailSignupDto.FormId}#signup:{emailSignupDto.Email}";
        var encodedValue = Encoding.UTF8.GetBytes(signupCandidateValue);
        var confirmationToken = Guid.NewGuid().ToString();

        _logger.LogInformation($"Storing email signup candidate in cache with token {confirmationToken}.");

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
        };
        await _signupCandidatesCache.SetAsync(confirmationToken, encodedValue, options);

        var message = new Message(new string[] { emailSignupDto.Email }, $"Confirm Signup to {form.FormName}", CreateHtmlContentFromTemplate(confirmationToken));
        var smtpConfiguration = await _smtpEmailSettingsRepository.GetByIdAsync(form.Id);
        _emailSender.SendEmail(message, smtpConfiguration);

        _logger.LogInformation("Email confirmation email sent.");

        return await Task.FromResult(new SignupResultDto
        {
            Success = true,
            Message = "Email address submitted, please confirm signup.",
        });
    }

    public async Task<ConfirmEmailResultDto> ConfirmEmailSignupAsync(string confirmationToken)
    {
        var encodedSignupCandidate = await _signupCandidatesCache.GetAsync(confirmationToken);

        if (encodedSignupCandidate == null)
        {
            return await Task.FromResult(new ConfirmEmailResultDto
            {
                Success = false,
                Message = "Confirmation token expired.",
                ErrorCode = EmailConfirmationErrorCode.ExpiredToken
            });
        }

        var signupCandidateValue = Encoding.UTF8.GetString(encodedSignupCandidate);

        var parts = signupCandidateValue.Split('#');
        var formIdPart = parts[0].Split(':')[1];
        var emailPart = parts[1].Split(':')[1];

        _logger.LogInformation($"Confirming email address {emailPart} for form {formIdPart}.");

        if (formIdPart == string.Empty || emailPart == string.Empty || !int.TryParse(formIdPart, out var formId))
        {
            return await Task.FromResult(new ConfirmEmailResultDto
            {
                Success = false,
                Message = "Invalid confirmation token.",
                ErrorCode = EmailConfirmationErrorCode.InvalidToken
            });
        }

        var allExistingSignups = await _emailSignupRepository.GetByFormIdAsync(formId);
        if (allExistingSignups.Any(s => s.EmailAddress == emailPart))
        {
            _logger.LogInformation("Email address already signed up.");
            return await Task.FromResult(new ConfirmEmailResultDto
            {
                Success = false,
                Message = "Email already confirmed.",
                ErrorCode = EmailConfirmationErrorCode.EmailAlreadyConfirmed
            });
        }

        var emailSignup = new EmailSignup
        {
            EmailAddress = emailPart,
            SignupFormId = formId,
        };

        await _emailSignupRepository.AddAsync(emailSignup);
        await _signupCandidatesCache.RemoveAsync(confirmationToken);

        return await Task.FromResult(new ConfirmEmailResultDto
        {
            Success = true,
            Message = "Email confirmed.",
        });
    }

    private bool ValidateEmail(string email)
    {
        _logger.LogInformation($"Validating email address {email}.");
        if (!EmailRegex.IsMatch(email))
        {
            _logger.LogInformation("Email address does not match regex.");
            return false;
        }

        return _dnsLookupService.HasMxRecords(email);
    }

    public async Task<IEnumerable<SignupStatsDto>> GetSignupsPerDayAsync(int formId, DateTime? startDate, DateTime? endDate)
    {
        var form = await _signupFormRepository.GetByIdAsync(formId);

        if (form == null)
        {
            throw new ArgumentException("Form not found.");
        }

        var rangeStart = startDate ?? form.CreatedAt;
        //if (rangeStart < form.CreatedAt) rangeStart = form.CreatedAt;

        var rangeEnd = endDate ?? DateTime.Now.Date;

        _logger.LogInformation($"Getting signups for form {form.Id} in range {rangeStart} - {rangeEnd}");
        var signups = await _emailSignupRepository.GetSignupsByFormIdAndDateRangeAsync(formId, rangeStart, rangeEnd);
        _logger.LogInformation($"Found {signups.Count()} signups for form {form.Id} in range {rangeStart} - {rangeEnd}");

        var filledSignups = FillMissingDates(signups, rangeStart, rangeEnd);

        return filledSignups;
    }

    private IEnumerable<SignupStatsDto> FillMissingDates(IEnumerable<SignupStatsDto> signups, DateTime startDate, DateTime endDate)
    {
        var filledSignups = new List<SignupStatsDto>();

        for (var date = startDate.Date; date <= endDate; date = date.AddDays(1))
        {
            var existingSignup = signups.FirstOrDefault(s => s.Date == date);
            filledSignups.Add(existingSignup ?? new SignupStatsDto { Date = date, Count = 0 });
        }

        return filledSignups;
    }

    private string CreateHtmlContentFromTemplate(string confirmationToken)
    {
        var templatePath = "./Services/EmailSender/EmailTemplate.html";
        var htmlContent = File.ReadAllText(templatePath);
        var emailBody = htmlContent.Replace("{{CONFIRMATION_TOKEN}}", confirmationToken)
            .Replace("{{COLLECTO_DOMAIN}}", Environment.GetEnvironmentVariable("COLLECTO_DOMAIN"));
        return emailBody;
    }
}
