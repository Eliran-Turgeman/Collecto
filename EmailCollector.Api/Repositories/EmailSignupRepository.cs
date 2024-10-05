using EmailCollector.Api.Data;
using EmailCollector.Domain.Entities;
using EmailCollector.Domain.Interfaces.Repositories;
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
}
