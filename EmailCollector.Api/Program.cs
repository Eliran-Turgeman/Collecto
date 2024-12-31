using System.Net;
using EmailCollector.Api.Data;
using Microsoft.EntityFrameworkCore;
using EmailCollector.Api.Repositories;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Authentication.Cookies;
using AspNetCoreRateLimit;
using EmailCollector.Api.Extensions;
using EmailCollector.Api.NotificationHandlers;


var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SendEmailOnSignupHandler).Assembly));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Identity/Account/Login";
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    });

builder.Services.AddForms();
builder.Services.AddApiKey();
builder.Services.AddEmailSender(builder.Configuration);
builder.Services.AddTemplateStorageProvider(builder.Configuration);
builder.Services.AddCache();
builder.Services.AddRateLimiting(builder.Configuration);
builder.Services.AddFeatureToggle(builder.Configuration);
builder.Services.AddApiOptions(builder.Configuration);
builder.Services.AddSwagger();

builder.Services.AddDbContext<EmailCollectorApiContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("EmailCollectorDB") ?? throw new InvalidOperationException("Connection string 'EmailCollectorDB' not found.")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddHttpClient();

builder.Services.AddAuthorization();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/configurations");
    options.Conventions.AuthorizeFolder("/ApiKeys");
    options.Conventions.AuthorizePage("/Forms");
    options.Conventions.AuthorizePage("/dashboard");
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
