using EmailCollector.Api.Data;
using EmailCollector.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailCollector.Api.Repositories;

public class SignupFormRepository : ISignupFormRepository
{
    private readonly EmailCollectorApiContext _dbContext;
    private readonly ILogger<SignupFormRepository> _logger;

    public SignupFormRepository(EmailCollectorApiContext dbContenxt, ILogger<SignupFormRepository> logger)
    {
        _dbContext = dbContenxt;
        _logger = logger;
    }

    public async Task AddAsync(SignupForm entity)
    {
        _dbContext.SignupForms.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<SignupForm?> GetByFormIdentifierAsync(Guid formIdentifier, Guid userId)
    {
        return await _dbContext.SignupForms
            .Where(form => form.CreatedBy == userId)
            .FirstOrDefaultAsync(form => form.Id == formIdentifier); ;
    }

    public async Task<IEnumerable<SignupForm>> GetByIds(IEnumerable<Guid> formIds)
    {
        return await _dbContext.SignupForms
            .Where(form => formIds.Contains(form.Id))
            .ToListAsync();
    }

    public async Task<SignupForm?> GetByIdAsync(object id)
    {
        return await _dbContext.SignupForms.FindAsync(id);
    }

    public async Task<IEnumerable<SignupForm>> GetByUserIdAsync(Guid userId)
    {
        return await _dbContext.SignupForms
            .Where(form => form.CreatedBy == userId)
            .ToListAsync();
    }

    public async Task Remove(SignupForm entity)
    {
        _dbContext.SignupForms.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(SignupForm entity)
    {
        _dbContext.SignupForms.Update(entity);
        await _dbContext.SaveChangesAsync();
    }
}
