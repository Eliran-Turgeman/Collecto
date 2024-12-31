using System.Text.Json.Serialization;
using EmailCollector.Domain.Enums;

namespace EmailCollector.Domain.Entities;

public abstract class FormEmailSettings
{
    public Guid FormId { get; set; }
    public EmailMethod EmailMethod { get; set; }
    public string EmailFrom { get; set; }
    
    [JsonIgnore]
    public SignupForm Form { get; set; }
}
