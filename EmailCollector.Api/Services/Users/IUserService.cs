using EmailCollector.Domain.Entities;

namespace EmailCollector.Api.Services.Users;

public interface IUserService
{
    public EmailCollectorApiUser ValidateApiKey(string apiKey);
}