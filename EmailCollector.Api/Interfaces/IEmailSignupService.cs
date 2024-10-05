using EmailCollector.Api.DTOs;

namespace EmailCollector.Api.Interfaces;

/// <summary>
/// Defines methods for handling email signups.
/// </summary>
public interface IEmailSignupService
{
    Task<SignupResultDto> SubmitEmailAsync(EmailSignupDto emailSignupDto);

    Task<IEnumerable<EmailSignupDto>?> GetSignupsByFormIdAsync(int formId, Guid userId);
}