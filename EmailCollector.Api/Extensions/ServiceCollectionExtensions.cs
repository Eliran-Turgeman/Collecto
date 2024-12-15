using System.Reflection;
using AspNetCoreRateLimit;
using EmailCollector.Api.Authentication;
using EmailCollector.Api.Configurations;
using EmailCollector.Api.Data;
using EmailCollector.Api.Middlewares;
using EmailCollector.Api.Repositories;
using EmailCollector.Api.Services;
using EmailCollector.Api.Services.CustomEmailTemplates;
using EmailCollector.Api.Services.CustomEmailTemplates.StorageProviders;
using EmailCollector.Api.Services.EmailSender;
using EmailCollector.Api.Services.Exports;
using EmailCollector.Api.Services.Exports.ExportStrategies;
using EmailCollector.Api.Services.Users;
using EmailCollector.Domain.Entities;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.OpenApi.Models;

namespace EmailCollector.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddTemplateStorageProvider(this IServiceCollection services, IConfiguration configuration)
    {
        var storageProviderType = configuration["TemplateStorage:Provider"];

        if (string.IsNullOrWhiteSpace(storageProviderType))
        {
            throw new InvalidOperationException("TemplateStorage:Provider configuration is missing.");
        }

        switch (storageProviderType)
        {
            case "FileSystem":
                var baseDirectory = configuration["TemplateStorage:FileSystem:BaseDirectory"];
                if (string.IsNullOrWhiteSpace(baseDirectory))
                {
                    throw new InvalidOperationException("TemplateStorage:FileSystem:BaseDirectory configuration is missing.");
                }

                services.AddSingleton<ITemplateStorageProvider>(_ =>
                    new FileSystemTemplateStorageProvider(baseDirectory));
                break;

            default:
                throw new InvalidOperationException($"Unsupported TemplateStorage provider: {storageProviderType}");
        }
    }

    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
         {
             options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
             {
                 In = ParameterLocation.Header,
                 Name = "x-api-key",
                 Type = SecuritySchemeType.ApiKey,
                 Scheme = "ApiKeySchema"
             });
             var scheme = new OpenApiSecurityScheme
             {
                 Reference = new OpenApiReference
                 {
                     Type = ReferenceType.SecurityScheme,
                     Id = "ApiKey"
                 },
                 In = ParameterLocation.Header
             };
             var requirement = new OpenApiSecurityRequirement
             {
                 { scheme, [] }
             };
             options.AddSecurityRequirement(requirement);
             
             options.SwaggerDoc("v1", new OpenApiInfo
             {
                 Version = "v1",
                 Title = "Collecto",
                 Description = "Email collection service",
                 TermsOfService = new Uri("https://example.com/terms"),
                 Contact = new OpenApiContact
                 {
                     Name = "Contact",
                     Url = new Uri("https://github.com/Eliran-Turgeman/Collecto")
                 },
                 License = new OpenApiLicense
                 {
                     Name = "AGPL-3.0 License",
                     Url = new Uri("https://github.com/Eliran-Turgeman/Collecto/blob/master/LICENSE")
                 }
             });
         
             var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
             options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
         });
    }

    public static void AddForms(this IServiceCollection services)
    {
        services.AddScoped<ISignupFormRepository, SignupFormRepository>();
        services.AddScoped<IFormService, FormService>();
        
        services.AddScoped<IEmailSignupRepository, EmailSignupRepository>();
        services.AddScoped<IEmailSignupService, EmailSignupService>();
        
        services.AddScoped(typeof(ExportMetadataResolver));
        services.AddScoped<IExportService, ExportService>();
        services.AddScoped<IExportFormatStrategy, CsvExportStrategy>();
        services.AddScoped<IExportFormatStrategy, JsonExportStrategy>();

        services.AddScoped<ICustomEmailTemplatesService, CustomEmailTemplatesService>();
        services.AddScoped<IFormRelatedRepository<CustomEmailTemplate>, FormRelatedRepository<CustomEmailTemplate>>();

    }

    public static void AddApiKey(this IServiceCollection services)
    {
        services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
        services.AddScoped<IApiKeyService, ApiKeyService>();
        
        services.AddScoped<ApiKeyAuthFilter>();

    }

    public static void AddEmailSender(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IEmailSender, EmailSender>();

        services.AddTransient<IAppEmailSender>(sp =>
        {
            // Resolve the IFeatureToggles service from the IServiceProvider
            var featureToggles = sp.GetRequiredService<IFeatureToggles>();
            var logger = sp.GetRequiredService<ILogger<EmailSender>>();
            if (featureToggles.IsEmailConfirmationEnabled())
            {
                // Return the real EmailSender if the feature is enabled
                var emailConfig = sp.GetService<EmailConfiguration>();
                return new EmailSender(emailConfig!, logger);  // Assuming EmailSender depends on EmailConfiguration
            }
            else
            {
                // Return the NoOpEmailSender if the feature is disabled
                return new NoOpAppEmailSender();
            }
        });
        
        // Register EmailConfiguration as a singleton if needed
        var emailConfig = configuration
            .GetSection("EmailConfiguration")
            .Get<EmailConfiguration>();

        if (emailConfig != null)
        {
            services.AddSingleton(emailConfig);
        }
    }

    public static void AddCache(this IServiceCollection services)
    {
        services.AddMemoryCache();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = Environment.GetEnvironmentVariable("Redis__ConnectionString") ?? "redis:6379";
        });
    }

    public static void AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddInMemoryRateLimiting();
    }

    public static void AddFeatureToggle(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FeatureToggles>(configuration.GetSection("FeatureToggles"));
        services.AddTransient<IFeatureToggles, FeatureTogglesService>();
    }

    public static void AddApiOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            var validDomains = configuration.GetSection("ValidCorsOrigins").Get<string>()?.Split(",") ?? [];
            options.AddPolicy("AllowSpecificOrigin",
                b => b.WithOrigins(validDomains)
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
        
        services.AddScoped<AllowedOriginsFilter>();
        
        services.AddIdentityApiEndpoints<EmailCollectorApiUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<EmailCollectorApiContext>();

        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

                var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
            };
        });
        
        services.AddEndpointsApiExplorer();
        services.AddSwagger();

    }
}