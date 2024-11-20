using System.Security.Cryptography;
using System.Text;
using EmailCollector.Api.Data;
using EmailCollector.Api.DTOs;
using EmailCollector.Api.Repositories;
using EmailCollector.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailCollector.Api.Services.Users;

public class ApiKeyService : IApiKeyService
{
    private readonly IApiKeyRepository _repository;
    private readonly EmailCollectorApiContext _context;

    public ApiKeyService(IApiKeyRepository repository, EmailCollectorApiContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<ApiKeyCreatedDto> GenerateApiKeyAsync(string userId, string name, DateTime? expiration = null)
    {
        var rawKey = Guid.NewGuid().ToString("N"); // raw api key without dashes
        var hashedKey = HashApiKey(rawKey);

        var apiKey = new ApiKey
        {
            KeyHash = hashedKey,
            UserId = userId,
            Name = name,
            CreatedAt = DateTime.UtcNow,
            Expiration = expiration,
            IsRevoked = false
        };

        await _repository.AddAsync(apiKey);

        return new ApiKeyCreatedDto
        {
            Id = apiKey.Id,
            ApiKey = rawKey,
        };
    }

    public async Task<EmailCollectorApiUser?> ValidateApiKeyAsync(string rawKey)
    {
        var hashedKey = HashApiKey(rawKey);

        var apiKey = await _context.ApiKeys
            .Include(k => k.User)
            .FirstOrDefaultAsync(k => k.KeyHash == hashedKey && !k.IsRevoked);

        return apiKey?.User;
    }

    public async Task RevokeApiKeyAsync(Guid id)
    {
        var apiKey = await _repository.GetByIdAsync(id);
        if (apiKey != null)
        {
            apiKey.IsRevoked = true;
            await _repository.Update(apiKey);
        }
    }

    public async Task<IEnumerable<ApiKeyDto>> GetAllByUserIdAsync(Guid userId)
    {
        var apiKeys = await _repository.GetAllByUserIdAsync(userId);
        return apiKeys.Select(k => new ApiKeyDto
        {
            Name = k.Name,
            CreatedAt = k.CreatedAt,
            Expiration = k.Expiration,
            IsRevoked = k.IsRevoked
        });
    }

    private string HashApiKey(string apiKey)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(apiKey));
        return Convert.ToBase64String(hash);
    }
}