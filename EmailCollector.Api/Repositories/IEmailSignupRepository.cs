using EmailCollector.Api.DTOs;
using EmailCollector.Domain.Entities;

namespace EmailCollector.Api.Repositories;

/// <summary>
/// Repository interface for EmailSignup entity.
/// </summary>
public interface IEmailSignupRepository : IRepository<EmailSignup>
{
    Task<IEnumerable<EmailSignup>> GetByFormIdAsync(int formId);

    Task<IEnumerable<SignupStatsDto>> GetSignupsByFormIdAndDateRangeAsync(int formId, DateTime rangeStart, DateTime rangeEnd);
}
