using EmailCollector.Api.Data;
using EmailCollector.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailCollector.Api.Repositories;

public class SignupFormRepository :  Repository<SignupForm>, ISignupFormRepository
{
    private readonly EmailCollectorApiContext _dbContext;

    public SignupFormRepository(EmailCollectorApiContext dbContenxt) : base(dbContenxt)
    {
        _dbContext = dbContenxt;
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

    public async Task<IEnumerable<SignupForm>> GetByUserIdAsync(Guid userId)
    {
        return await _dbContext.SignupForms
            .Where(form => form.CreatedBy == userId)
            .ToListAsync();
    }
}
