using System.Text.Json.Serialization;

namespace EmailCollector.Domain.Entities;

public class RecaptchaFormSettings
{
    public Guid FormId { get; set; }
    public string? SiteKey { get; set; }
    public string? SecretKey { get; set; }
    
    [JsonIgnore]
    public SignupForm Form { get; set; }
}