using EmailCollector.Api.Data;
using EmailCollector.Api.DTOs;
using EmailCollector.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailCollector.Api.Repositories;

public class EmailSignupRepository : IEmailSignupRepository
{
    private readonly EmailCollectorApiContext _dbContext;
    private readonly ILogger<EmailSignupRepository> _logger;
    private readonly ISignupFormRepository _signupFormRepository;

    public EmailSignupRepository(EmailCollectorApiContext dbContenxt,
        ILogger<EmailSignupRepository> logger,
        ISignupFormRepository signupFormRepository)
    {
        _dbContext = dbContenxt;
        _logger = logger;
        _signupFormRepository = signupFormRepository;
    }

    public async Task AddAsync(EmailSignup entity)
    {
        _dbContext.EmailSignups.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<EmailSignup>> GetByFormIdAsync(Guid formId)
    {
        return await _dbContext.EmailSignups
            .Where(signup => signup.SignupFormId == formId)
            .ToListAsync();
    }

    public Task<EmailSignup?> GetByIdAsync(object id)
    {
        throw new NotImplementedException();
    }

    public async Task Remove(EmailSignup entity)
    {
        _dbContext.EmailSignups.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(EmailSignup entity)
    {
        _dbContext.EmailSignups.Update(entity);
        await _dbContext.SaveChangesAsync();
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
