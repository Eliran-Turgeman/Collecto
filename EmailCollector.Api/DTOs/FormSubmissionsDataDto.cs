using EmailCollector.Api.Services.Exports;

namespace EmailCollector.Api.DTOs;

public class FormSubmissionsDataDto : IExportable
{
    [ExportField("Form ID")]
    public Guid Id { get; set; }
    
    [ExportField("Form Name")]
    public required string FormName { get; set; }
    
    [ExportField("Subscriber Email")]
    public IEnumerable<string> Emails { get; set; }
}