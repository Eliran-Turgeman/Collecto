using System.Text.Json.Serialization;
using EmailCollector.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EmailCollector.Domain.Entities;

[Index(nameof(FormId), nameof(Event), IsUnique = true, Name = "IX_CustomEmailTemplate_FormId_Event_Unique")]
public class CustomEmailTemplate
{
    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public TemplateEvent Event { get; set; }
    public string TemplateSubject { get; set; }
    public string TemplateBodyUri  { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    [JsonIgnore]
    public SignupForm Form { get; set; }

}