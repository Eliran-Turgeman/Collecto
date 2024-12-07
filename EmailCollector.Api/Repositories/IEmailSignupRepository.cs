using EmailCollector.Api.DTOs;
using EmailCollector.Domain.Entities;

namespace EmailCollector.Api.Repositories;

/// <summary>
/// Repository interface for EmailSignup entity.
/// </summary>
public interface IEmailSignupRepository : IRepository<EmailSignup>
{
    Task<IEnumerable<EmailSignup>> GetByFormIdAsync(Guid formId);

    Task<IEnumerable<SignupStatsDto>> GetSignupsByFormIdAndDateRangeAsync(Guid formId, DateTime rangeStart, DateTime rangeEnd);

    Task<int> GetSignupCountByFormId(Guid formId);
}
