using EmailCollector.Api.DTOs;
using EmailCollector.Domain.Enums;

namespace EmailCollector.Api.Services.CustomEmailTemplates;

public interface ICustomEmailTemplatesService
{
    Task<IEnumerable<CustomEmailTemplateDto>> GetCustomEmailTemplatesByFormId(Guid formId);
    Task<Dictionary<Guid, IEnumerable<CustomEmailTemplateDto>>> GetCustomEmailTemplatesByFormIds(IEnumerable<Guid> formIds);
    Task<CustomEmailTemplateDto?> GetCustomEmailTemplateByFormIdAndEvent(Guid formId, TemplateEvent templateEvent);
    Task<Guid> SaveCustomEmailTemplate(CustomEmailTemplateDto customEmailTemplateDto);
    Task DeleteCustomEmailTemplate(Guid templateId);
    Task <CustomEmailTemplateDto> GetCustomEmailTemplateById(Guid id);
}
