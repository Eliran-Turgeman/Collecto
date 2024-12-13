using EmailCollector.Api.Data;
using EmailCollector.Api.DTOs;
using EmailCollector.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailCollector.Api.Repositories;

public class EmailSignupRepository : Repository<EmailSignup>, IEmailSignupRepository
{
    private readonly EmailCollectorApiContext _dbContext;

    public EmailSignupRepository(EmailCollectorApiContext dbContenxt) : base(dbContenxt)
    {
        _dbContext = dbContenxt;
    }

    public async Task<IEnumerable<EmailSignup>> GetByFormIdAsync(Guid formId)
    {
        return await _dbContext.EmailSignups
            .Where(signup => signup.SignupFormId == formId)
            .ToListAsync();
    }

    public async Task<IEnumerable<SignupStatsDto>> GetSignupsByFormIdAndDateRangeAsync(Guid formId, DateTime rangeStart, DateTime rangeEnd)
    {
        // Fetch the signups grouped by date
        var signups = await _dbContext.EmailSignups
            .Where(s => s.SignupFormId == formId && s.SignupDate.Date >= rangeStart.Date && s.SignupDate.Date <= rangeEnd.Date)
            .GroupBy(s => s.SignupDate.Date)
            .Select(g => new SignupStatsDto
            {
                Date = g.Key,
                Count = g.Count()
            })
            .ToListAsync();

        return signups;
    }

    /// <summary>
    /// Retrieves the number of signups for a form.
    /// </summary>
    /// <param name="formId">The form identifer</param>
    /// <returns>Count of all-time signups for form id specified</returns>
    public async Task<int> GetSignupCountByFormId(Guid formId)
    {
        return await _dbContext.EmailSignups
            .Where(signup => signup.SignupFormId == formId)
            .CountAsync();
    }
}
