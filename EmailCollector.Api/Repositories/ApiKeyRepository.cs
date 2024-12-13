using EmailCollector.Api.Data;
using EmailCollector.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailCollector.Api.Repositories;

public class ApiKeyRepository : Repository<ApiKey>, IApiKeyRepository
{
    private readonly EmailCollectorApiContext _context;

    public ApiKeyRepository(EmailCollectorApiContext context): base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ApiKey>> GetAllByUserIdAsync(Guid userId)
    {
        return await _context.ApiKeys
            .Where(s => s.User.Id == userId.ToString())
            .ToListAsync();
    }
}