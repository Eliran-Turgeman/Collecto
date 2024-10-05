using System.Security.Claims;

namespace EmailCollector.Api.Middlewares;

public class UserMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserMiddleware> _logger;

    public UserMiddleware(RequestDelegate next, ILogger<UserMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                context.Items["UserId"] = userId;

            }
            _logger.LogInformation($"Authenticated User ID: {userId}");
        }
        await _next(context);
    }
}
