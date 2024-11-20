using EmailCollector.Domain.Entities;

namespace EmailCollector.Api.Repositories;

public interface IApiKeyRepository : IRepository<ApiKey>
{
    Task<IEnumerable<ApiKey>> GetAllByUserIdAsync(Guid userId);
}