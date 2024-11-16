using System.Text;
using System.Text.Json;
using EmailCollector.Api.DTOs;
using EmailCollector.Api.Repositories;
using EmailCollector.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmailCollector.Api.Middlewares;

public class AllowedOriginsFilter : IAsyncAuthorizationFilter
{
    private readonly ILogger<AllowedOriginsFilter> _logger;
    private readonly IFormCorsSettingsRepository _formCorsSettingsRepository;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public AllowedOriginsFilter(
        ILogger<AllowedOriginsFilter> logger,
        IFormCorsSettingsRepository formCorsSettingsRepository)
    {
        _logger = logger;
        _formCorsSettingsRepository = formCorsSettingsRepository;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var httpContext = context.HttpContext;
        httpContext.Request.Headers.TryGetValue("Origin", out var origin);
        httpContext.Request.EnableBuffering();

        try
        {
            var emailSignup = await DeserializeRequestBodyAsync<EmailSignupDto>(httpContext.Request);

            if (emailSignup != null)
            {
                var formCorsSettings = await _formCorsSettingsRepository.GetByIdAsync(emailSignup.FormId);
                if (formCorsSettings == null || !IsOriginAllowed(origin, formCorsSettings))
                {
                    _logger.LogWarning($"Origin {origin} is not allowed for form {emailSignup?.FormId}.");
                    httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Result = new UnauthorizedObjectResult("Origin is not allowed.");
                    return;
                }

                _logger.LogInformation($"Origin {origin} is allowed for form {emailSignup.FormId}.");
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError($"Failed to deserialize request body. Error: {ex.Message}");
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Result = new BadRequestResult();
        }
    }

    private async Task<T?> DeserializeRequestBodyAsync<T>(HttpRequest request)
    {
        request.Body.Position = 0; // Ensure the stream is at the beginning

        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var requestBody = await reader.ReadToEndAsync();

        _logger.LogInformation("Request Body: {RequestBody}", requestBody);

        request.Body.Position = 0; // Reset stream for further usage
        return JsonSerializer.Deserialize<T>(requestBody, _jsonSerializerOptions);
    }

    private bool IsOriginAllowed(string? origin, FormCorsSettings formCorsSettings)
    {
        return string.IsNullOrEmpty(origin) || formCorsSettings.AllowedOrigins.Contains(origin!);
    }
}