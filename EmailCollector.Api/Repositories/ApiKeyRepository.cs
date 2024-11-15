using EmailCollector.Api.Data;
using EmailCollector.Domain.Entities;

namespace EmailCollector.Api.Repositories;

public class ApiKeyRepository : IApiKeyRepository
{
    private readonly EmailCollectorApiContext _context;

    public ApiKeyRepository(EmailCollectorApiContext context)
    {
        _context = context;
    }
    
    public async Task<ApiKey?> GetByIdAsync(object id)
    {
        return await _context.ApiKeys.FindAsync(id);
    }

    public async Task AddAsync(ApiKey entity)
    {
        _context.ApiKeys.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Update(ApiKey entity)
    {
        _context.ApiKeys.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Remove(ApiKey entity)
    {
        _context.ApiKeys.Remove(entity);
        await _context.SaveChangesAsync();
    }
}