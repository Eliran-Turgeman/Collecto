namespace EmailCollector.Application.DTOs;

/// <summary>
/// DTO representing the result of a signup attempt.
/// </summary>
public class SignupResultDto
{
    public bool Success { get; set; }

    public required string Message { get; set; }
}