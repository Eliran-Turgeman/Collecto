using EmailCollector.Api.Data;
using Microsoft.EntityFrameworkCore;
using EmailCollector.Api.Areas.Identity.Data;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using EmailCollector.Api.Middlewares;
using EmailCollector.Api.Services;
using EmailCollector.Domain.Interfaces.Repositories;
using EmailCollector.Api.Repositories;
using System.Reflection;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Authentication.Cookies;
using EmailCollector.Api.Pages;
using AspNetCoreRateLimit;
using EmailCollector.Api.Services.EmailSender;
using EmailCollector.Api.Configurations;
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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Email Collection API",
        Description = "API for managing email collection forms.",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddScoped<ISignupFormRepository, SignupFormRepository>();
builder.Services.AddScoped<IFormService, FormService>();

builder.Services.AddScoped<IEmailSignupRepository, EmailSignupRepository>();
builder.Services.AddScoped<IEmailSignupService, EmailSignupService>();

builder.Services.AddSingleton<IDnsLookupService, DnsLookupService>();

builder.Services.AddDbContext<EmailCollectorApiContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("EmailCollectorDB") ?? throw new InvalidOperationException("Connection string 'EmailCollectorDB' not found.")));

builder.Services.AddHttpClient();

builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Identity/Account/Login";
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    });

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
    Console.WriteLine($"ValidCorsOrigins: {string.Join(", ", validDomains)}");
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins(validDomains)
                              .AllowAnyMethod()
                              .AllowAnyHeader());
});

builder.Services.AddRazorPages();

//added to use in-memory cache
builder.Services.AddMemoryCache();

//added to use Redis cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = Environment.GetEnvironmentVariable("Redis__ConnectionString") ?? "redis:6379";
});

var app = builder.Build();

app.UseStaticFiles();
app.UseIpRateLimiting();

app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EmailCollectorApi");
    c.InjectStylesheet("/swagger-ui/darkMode.css");
});
//app.MapIdentityApi<EmailCollectorApiUser>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseAntiforgery();
app.UseAuthentication();
app.UseMiddleware<UserMiddleware>();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapRazorComponents<Dashboard>();
});
app.MapControllers();

app.Run();
