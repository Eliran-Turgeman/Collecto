using EmailCollector.Domain.Enums;

namespace EmailCollector.Api.DTOs;

public class CustomEmailTemplateDto
{
    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public TemplateEvent Event { get; set; }
    public string TemplateSubject { get; set; }
    public string TemplateBodyUri  { get; set; }
    public string TemplateBody { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}