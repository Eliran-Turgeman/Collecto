using EmailCollector.Application.DTOs;

namespace EmailCollector.Application.Interfaces;

/// <summary>
/// Defines methods for handling email signups.
/// </summary>
public interface ISignupService
{
    Task<SignupResultDto> SubmitEmailAsync(EmailSignupDto emailSignupDto);
}