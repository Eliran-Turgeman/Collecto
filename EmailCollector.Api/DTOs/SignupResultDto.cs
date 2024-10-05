using EmailCollector.Domain.Enums;

namespace EmailCollector.Api.DTOs;

/// <summary>
/// DTO representing the result of a signup attempt.
/// </summary>
public class SignupResultDto
{
    public bool Success { get; set; }

    public required string Message { get; set; }

    public EmailSignupErrorCode? ErrorCode { get; set; }
}