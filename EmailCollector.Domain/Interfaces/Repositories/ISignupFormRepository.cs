using EmailCollector.Domain.Entities;

namespace EmailCollector.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for SignupForm entity.
/// </summary>
public interface ISignupFormRepository : IRepository<SignupForm>
{
    Task<IEnumerable<SignupForm>> GetByUserIdAsync(string userId);

    Task<SignupForm> GetByFormIdentifierAsync(string formIdentifier);

    Task<bool> IsDomainAllowedAsync(int formId, string domain);
}
