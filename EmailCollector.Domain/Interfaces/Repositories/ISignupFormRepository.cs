using EmailCollector.Domain.Entities;

namespace EmailCollector.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for SignupForm entity.
/// </summary>
public interface ISignupFormRepository : IRepository<SignupForm>
{
    Task<IEnumerable<SignupForm>> GetByUserIdAsync(Guid userId);

    Task<SignupForm?> GetByFormIdentifierAsync(int formIdentifier, Guid userId);
}
