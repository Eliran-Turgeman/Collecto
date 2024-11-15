using EmailCollector.Api.DTOs;

namespace EmailCollector.Api.Services;

/// <summary>
/// Defines methods for handling email signups.
/// </summary>
public interface IEmailSignupService
{
    Task<SignupResultDto> SubmitEmailAsync(EmailSignupDto emailSignupDto);

    Task<IEnumerable<EmailSignupDto>?> GetSignupsByFormIdAsync(int formId);

    Task<IEnumerable<SignupStatsDto>> GetSignupsPerDayAsync(int formId, DateTime? startDate, DateTime? endDate);

    Task<ConfirmEmailResultDto> ConfirmEmailSignupAsync(string confirmationToken);
}