using EmailCollector.Api.Data;
using Microsoft.EntityFrameworkCore;
using EmailCollector.Api.Areas.Identity.Data;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using EmailCollector.Api.Middlewares;
using EmailCollector.Api.Interfaces;
using EmailCollector.Api.Services;
using EmailCollector.Domain.Interfaces.Repositories;
using EmailCollector.Api.Repositories;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<EmailCollectorApiUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<EmailCollectorApiContext>();

var app = builder.Build();

app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EmailCollectorApi");
        c.InjectStylesheet("/swagger-ui/darkMode.css");
    });
}
app.MapIdentityApi<EmailCollectorApiUser>();
app.UseMiddleware<UserMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.Run();
