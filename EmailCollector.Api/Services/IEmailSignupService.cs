using EmailCollector.Api.DTOs;

namespace EmailCollector.Api.Services;

/// <summary>
/// Defines methods for handling email signups.
/// </summary>
public interface IEmailSignupService
{
    Task<SignupResultDto> SubmitEmailAsync(EmailSignupDto emailSignupDto);

    Task<IEnumerable<EmailSignupDto>?> GetSignupsByFormIdAsync(Guid formId);

    Task<IEnumerable<SignupStatsDto>> GetSignupsPerDayAsync(Guid formId, DateTime? startDate, DateTime? endDate);

    Task<ConfirmEmailResultDto> ConfirmEmailSignupAsync(string confirmationToken);
}