using EmailCollector.Domain.Entities;

namespace EmailCollector.Domain.Interfaces.Services;

/// <summary>
/// Service interface for managing signup forms.
/// </summary>
public interface ISignupFormService
{
    Task<SignupForm> CreateFormAsync(string userId, string formName, IEnumerable<string> allowedDomains);

    Task<IEnumerable<SignupForm>> GetFormsByUserAsync(string userId);

    Task<SignupForm> GetFormByIdAsync(int formId, string userId);

    Task<bool> IsDomainAllowedAsync(int formId, string domain);
}
