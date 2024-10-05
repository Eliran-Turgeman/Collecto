namespace EmailCollector.Domain.Interfaces.Services;

/// <summary>
/// Service interface for email validation logic.
/// </summary>
public interface IEmailValidationService
{
    bool IsValidEmail(string email);
}
