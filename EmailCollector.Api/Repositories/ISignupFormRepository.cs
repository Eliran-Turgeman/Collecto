using EmailCollector.Domain.Entities;

namespace EmailCollector.Api.Repositories;

/// <summary>
/// Repository interface for SignupForm entity.
/// </summary>
public interface ISignupFormRepository : IRepository<SignupForm>
{
    Task<IEnumerable<SignupForm>> GetByUserIdAsync(Guid userId);

    Task<SignupForm?> GetByFormIdentifierAsync(Guid formIdentifier, Guid userId);
    
    Task<IEnumerable<SignupForm>> GetByIds(IEnumerable<Guid> formIds);
}
