using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EmailCollector.Api.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmailCollector.Api.Authentication;

public class ApiKeyAuthFilter : IAuthorizationFilter
{
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;

    public ApiKeyAuthFilter(IConfiguration configuration, IUserService userService)
    {
        _configuration = configuration;
        _userService = userService;
    }
    
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var apiKeyHeader))
        {
            context.Result = new UnauthorizedObjectResult("Api Key is missing");
            return;
        }
        
        var apiKey = _configuration.GetValue<string>(AuthConstants.ApiKeySectionName);
        
        // Using FixedTimeEquals instead of equals -> "This method compares the contents from two
        // buffers for equality in a way that doesn't leak timing information,
        // making it ideal for use within cryptographic routines."
        // https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.cryptographicoperations.fixedtimeequals?view=net-8.0
        if (!CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(apiKey!),
                Encoding.UTF8.GetBytes(apiKeyHeader!)))
        {
            context.Result = new UnauthorizedObjectResult("Api Key is invalid");
        };
        
        var user = _userService.ValidateApiKey(apiKeyHeader!);
        if (user == null)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
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