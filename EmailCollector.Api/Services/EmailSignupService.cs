using DnsClient;
using EmailCollector.Api.DTOs;
using EmailCollector.Api.Interfaces;
using EmailCollector.Domain.Entities;
using EmailCollector.Domain.Enums;
using EmailCollector.Domain.Interfaces.Repositories;
using System.Text.RegularExpressions;

namespace EmailCollector.Api.Services;

public class EmailSignupService : IEmailSignupService
{
    private readonly IEmailSignupRepository _emailSignupRepository;
    private readonly ISignupFormRepository _signupFormRepository;
    private readonly ILogger<EmailSignupService> _logger;
    private readonly IDnsLookupService _dnsLookupService;

    private static readonly Regex EmailRegex = new Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public EmailSignupService(IEmailSignupRepository emailSignupRepository,
        ISignupFormRepository signupFormRepository,
        ILogger<EmailSignupService> logger,
        IDnsLookupService dnsLookupService)
    {
        _emailSignupRepository = emailSignupRepository;
        _signupFormRepository = signupFormRepository;
        _logger = logger;
        _dnsLookupService = dnsLookupService;
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
            return await Task.FromResult(new SignupResultDto
            {
                Success = false,
                Message = "Form not found.",
                ErrorCode = EmailSignupErrorCode.FormNotFound,
            });
        }

        if (form.Status != FormStatus.Active)
        {
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

        await _emailSignupRepository.AddAsync(emailSignup);

        return await Task.FromResult(new SignupResultDto
        {
            Success = true,
            Message = "Email address submitted successfully.",
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
}
