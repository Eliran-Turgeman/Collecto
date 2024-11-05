using Blazorise.Extensions;
using EmailCollector.Api.DTOs;
using EmailCollector.Api.Repositories;
using System.Text;
using System.Text.Json;

namespace EmailCollector.Api.Middlewares;

public class AllowedOriginsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AllowedOriginsMiddleware> _logger;
    private readonly IFormCorsSettingsRepository _formCorsSettingsRepository;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public AllowedOriginsMiddleware(RequestDelegate next, ILogger<AllowedOriginsMiddleware> logger, IFormCorsSettingsRepository formCorsSettingsRepository)
    {
        _next = next;
        _logger = logger;
        _formCorsSettingsRepository = formCorsSettingsRepository;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/api/EmailSignups") &&
            context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
        {
            var origin = context.Request.Headers["Origin"].FirstOrDefault();

            context.Request.EnableBuffering();

            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                var requestBody = await reader.ReadToEndAsync();

                _logger.LogInformation("Request Body: {RequestBody}", requestBody);

                // Reset the request body stream position so the next component can read it
                context.Request.Body.Position = 0;

                try
                {
                    var emailSignup = JsonSerializer.Deserialize<EmailSignupDto>(requestBody, _jsonSerializerOptions);

                    if (emailSignup != null)
                    {
                        var formCorsSettings = await _formCorsSettingsRepository.GetByIdAsync(emailSignup.FormId);

                        if (formCorsSettings != null)
                        {
                            if (origin.IsNullOrEmpty() || formCorsSettings.AllowedOrigins.Contains(origin!))
                            {
                                _logger.LogInformation($"Origin {origin} is allowed for form {emailSignup.FormId}.");
                            }
                            else
                            {
                                _logger.LogWarning($"Origin {origin} is not allowed for form {emailSignup.FormId}.");
                                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                                return;
                            }
                        }
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"Failed to deserialize request body. Error: {ex.Message}");
                }
            }
        }
        await _next(context);
    }
}
