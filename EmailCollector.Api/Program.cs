using System.Net;
using EmailCollector.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using EmailCollector.Api.Middlewares;
using EmailCollector.Api.Services;
using EmailCollector.Api.Repositories;
using System.Reflection;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Authentication.Cookies;
using AspNetCoreRateLimit;
using EmailCollector.Api.Authentication;
using EmailCollector.Api.Services.EmailSender;
using EmailCollector.Api.Configurations;
using EmailCollector.Api.NotificationHandlers;
using EmailCollector.Api.Services.Exports;
using EmailCollector.Api.Services.Exports.ExportStrategies;
using EmailCollector.Api.Services.Users;
using EmailCollector.Domain.Entities;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity.UI.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Serialize enums as strings
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
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

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SendEmailOnSignupHandler).Assembly));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Identity/Account/Login";
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    });

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

        var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
    };
});

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddScoped<ISignupFormRepository, SignupFormRepository>();
builder.Services.AddScoped<IFormService, FormService>();

builder.Services.AddScoped<IEmailSignupRepository, EmailSignupRepository>();
builder.Services.AddScoped<IEmailSignupService, EmailSignupService>();

builder.Services.AddScoped<ISmtpEmailSettingsRepository, SmtpEmailSettingsRepository>();
builder.Services.AddScoped<IFormCorsSettingsRepository, FormCorsSettingsRepository>();

builder.Services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();

builder.Services.AddScoped(typeof(ExportMetadataResolver));
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<IExportFormatStrategy, CsvExportStrategy>();
builder.Services.AddScoped<IExportFormatStrategy, JsonExportStrategy>();


builder.Services.AddDbContext<EmailCollectorApiContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("EmailCollectorDB") ?? throw new InvalidOperationException("Connection string 'EmailCollectorDB' not found.")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddHttpClient();

builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<EmailCollectorApiUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<EmailCollectorApiContext>();


builder.Services.Configure<FeatureToggles>(builder.Configuration.GetSection("FeatureToggles"));
builder.Services.AddTransient<IFeatureToggles, FeatureTogglesService>();

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddTransient<IAppEmailSender>(sp =>
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
var emailConfig = builder.Configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();

if (emailConfig != null)
{
    builder.Services.AddSingleton(emailConfig);
}

builder.Services.AddCors(options =>
{
    var validDomains = builder.Configuration.GetSection("ValidCorsOrigins").Get<string>()?.Split(",") ?? [];
    options.AddPolicy("AllowSpecificOrigin",
        b => b.WithOrigins(validDomains)
                              .AllowAnyMethod()
                              .AllowAnyHeader());
});

builder.Services.AddScoped<ApiKeyAuthFilter>();
builder.Services.AddScoped<AllowedOriginsFilter>();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/configurations");
    options.Conventions.AuthorizeFolder("/ApiKeys");
    options.Conventions.AuthorizePage("/Forms");
    options.Conventions.AuthorizePage("/dashboard");
});

//added to use in-memory cache
builder.Services.AddMemoryCache();

//added to use Redis cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = Environment.GetEnvironmentVariable("Redis__ConnectionString") ?? "redis:6379";
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EmailCollectorApiContext>();
    dbContext.Database.Migrate();
}

app.UseStaticFiles();
app.UseIpRateLimiting();

app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EmailCollectorApi");
    c.InjectStylesheet("/swagger-ui/darkMode.css");
});

app.UseStatusCodePages(async contextAccessor => 
{
    var response = contextAccessor.HttpContext.Response;

    if (response.StatusCode == (int)HttpStatusCode.Unauthorized || 
        response.StatusCode == (int)HttpStatusCode.Forbidden)
    {
        response.Redirect("/Identity/Account/Login");
    }
});

app.UseRouting();
app.UseCors();
app.UseAntiforgery();
app.UseAuthentication();
//middlewares go here
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
});
app.MapControllers();

app.Run();
