using System.Text.Json.Serialization;

namespace EmailCollector.Domain.Entities;

public class FormCorsSettings
{
    public Guid FormId { get; set; }
    public string AllowedOrigins { get; set; }
    
    [JsonIgnore]
    public SignupForm Form { get; set; }
}
