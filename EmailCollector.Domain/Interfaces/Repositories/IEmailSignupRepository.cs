using EmailCollector.Domain.Entities;

namespace EmailCollector.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for EmailSignup entity.
/// </summary>
public interface IEmailSignupRepository : IRepository<EmailSignup>
{
    Task<IEnumerable<EmailSignup>> GetByFormIdAsync(int formId);
}
