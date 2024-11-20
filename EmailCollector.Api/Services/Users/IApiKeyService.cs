using EmailCollector.Api.DTOs;
using EmailCollector.Domain.Entities;

namespace EmailCollector.Api.Services.Users;

public interface IApiKeyService
{
    Task<ApiKeyCreatedDto> GenerateApiKeyAsync(Guid userId, string name, DateTime? expiration = null);
    Task<EmailCollectorApiUser?> ValidateApiKeyAsync(string rawKey);
    Task RevokeApiKeyAsync(Guid id);
    Task<IEnumerable<ApiKeyDto>> GetAllByUserIdAsync(Guid userId);
}