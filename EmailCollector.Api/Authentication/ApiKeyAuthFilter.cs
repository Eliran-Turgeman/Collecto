using System.Security.Claims;
using EmailCollector.Api.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmailCollector.Api.Authentication;

public class ApiKeyAuthFilter : IAsyncAuthorizationFilter
{
    private readonly IApiKeyService _apiKeyService;

    public ApiKeyAuthFilter(IApiKeyService apiKeyService)
    {
        _apiKeyService = apiKeyService;
    }
    
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var apiKeyHeader))
        {
            context.Result = new UnauthorizedObjectResult("Api Key is missing");
            return;
        }
        
        var user = await _apiKeyService.ValidateApiKeyAsync(apiKeyHeader!);
        if (user == null)
        {
            context.Result = new UnauthorizedObjectResult("Api Key is invalid");
            return;
        }

        // Add user identity to HttpContext.User
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.NameIdentifier, user.Id),
            new("ApiKey", apiKeyHeader!)
        };

        var identity = new ClaimsIdentity(claims, "ApiKey");
        context.HttpContext.User = new ClaimsPrincipal(identity);
    }
}