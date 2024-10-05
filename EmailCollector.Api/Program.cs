using EmailCollector.Api.Data;
using Microsoft.EntityFrameworkCore;
using EmailCollector.Api.Areas.Identity.Data;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

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
});

builder.Services.AddDbContext<EmailCollectorApiContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("EmailCollectorDB") ?? throw new InvalidOperationException("Connection string 'EmailCollectorDB' not found.")));

builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<EmailCollectorApiUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<EmailCollectorApiContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapIdentityApi<EmailCollectorApiUser>();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.Run();
