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

    public async Task<IEnumerable<EmailSignup>> GetByFormIdAsync(int formId)
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

    public async Task<IEnumerable<SignupStatsDto>> GetSignupsByFormIdAndDateRangeAsync(int formId, DateTime rangeStart, DateTime rangeEnd)
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
}
