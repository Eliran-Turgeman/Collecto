using EmailCollector.Api.DTOs;
using EmailCollector.Api.Repositories;
using EmailCollector.Api.Services.CustomEmailTemplates.StorageProviders;
using EmailCollector.Domain.Entities;
using EmailCollector.Domain.Enums;

namespace EmailCollector.Api.Services.CustomEmailTemplates;

public class CustomEmailTemplatesService : ICustomEmailTemplatesService
{
    private IFormRelatedRepository<CustomEmailTemplate> _customEmailTemplatesRepository;
    private readonly ITemplateStorageProvider _templateStorageProvider;


    public CustomEmailTemplatesService(IFormRelatedRepository<CustomEmailTemplate> customEmailTemplatesRepository,
        ITemplateStorageProvider templateStorageProvider)
    {
        _customEmailTemplatesRepository = customEmailTemplatesRepository;
        _templateStorageProvider = templateStorageProvider;
    }
    
    public async Task<IEnumerable<CustomEmailTemplateDto>> GetCustomEmailTemplatesByFormId(Guid formId)
    {
        var customEmailTemplates = await _customEmailTemplatesRepository.GetByFormId(formId);

        List<CustomEmailTemplateDto> templates = [];

        foreach (CustomEmailTemplate customEmailTemplate in customEmailTemplates)
        {
            var templateBody = await _templateStorageProvider.GetTemplateBodyAsync(customEmailTemplate.TemplateBodyUri);
            templates.Add(new CustomEmailTemplateDto
            {
                Id = customEmailTemplate.Id,
                CreatedAt = customEmailTemplate.CreatedAt,
                Event = customEmailTemplate.Event,
                FormId = customEmailTemplate.FormId,
                IsActive = customEmailTemplate.IsActive,
                TemplateSubject = customEmailTemplate.TemplateSubject,
                TemplateBodyUri = customEmailTemplate.TemplateBodyUri,
                TemplateBody = templateBody,
                UpdatedAt = customEmailTemplate.UpdatedAt
            });
        }
        
        return templates;
    }

    public async Task<Dictionary<Guid, IEnumerable<CustomEmailTemplateDto>>> GetCustomEmailTemplatesByFormIds(IEnumerable<Guid> formIds)
    {
        if (formIds == null)
            throw new ArgumentNullException(nameof(formIds));

        var formIdsList = formIds.ToList();

        if (!formIdsList.Any())
            return new Dictionary<Guid, IEnumerable<CustomEmailTemplateDto>>();

        var entitiesByFormId = await _customEmailTemplatesRepository.GetByFormIds(formIdsList);

        var dtoDictionary = new Dictionary<Guid, IEnumerable<CustomEmailTemplateDto>>();

        foreach (var formId in formIdsList)
        {
            if (entitiesByFormId.TryGetValue(formId, out var entities) && entities.Any())
            {
                List<CustomEmailTemplateDto> templates = [];

                foreach (CustomEmailTemplate customEmailTemplate in entities)
                {
                    var templateBody = await _templateStorageProvider.GetTemplateBodyAsync(customEmailTemplate.TemplateBodyUri);
                    templates.Add(new CustomEmailTemplateDto
                    {
                        Id = customEmailTemplate.Id,
                        CreatedAt = customEmailTemplate.CreatedAt,
                        Event = customEmailTemplate.Event,
                        FormId = customEmailTemplate.FormId,
                        IsActive = customEmailTemplate.IsActive,
                        TemplateSubject = customEmailTemplate.TemplateSubject,
                        TemplateBodyUri = customEmailTemplate.TemplateBodyUri,
                        TemplateBody = templateBody,
                        UpdatedAt = customEmailTemplate.UpdatedAt
                    });
                }

                dtoDictionary[formId] = templates;
            }
            else
            {
                // Ensure that even FormIds with no templates are represented with an empty collection
                dtoDictionary[formId] = [];
            }
        }

        return dtoDictionary;
    }

    public async Task<CustomEmailTemplateDto?> GetCustomEmailTemplateByFormIdAndEvent(Guid formId, TemplateEvent templateEvent)
    {
        var templatesForForm = await GetCustomEmailTemplatesByFormId(formId);
        return templatesForForm
            .FirstOrDefault(t => t.Event == templateEvent);
    }

    public async Task SaveCustomEmailTemplate(CustomEmailTemplateDto customEmailTemplateDto)
    {
        if (customEmailTemplateDto == null)
            throw new ArgumentNullException(nameof(customEmailTemplateDto));

        string newUri = await _templateStorageProvider.SaveTemplateBodyAsync(customEmailTemplateDto.TemplateBody, 
            customEmailTemplateDto.TemplateBodyUri);

        var templateEntity = new CustomEmailTemplate
        {
            Id = customEmailTemplateDto.Id == Guid.Empty ? Guid.NewGuid() : customEmailTemplateDto.Id,
            CreatedAt = customEmailTemplateDto.Id == Guid.Empty ? DateTime.UtcNow : customEmailTemplateDto.CreatedAt,
            Event = customEmailTemplateDto.Event,
            FormId = customEmailTemplateDto.FormId,
            IsActive = customEmailTemplateDto.IsActive,
            TemplateSubject = customEmailTemplateDto.TemplateSubject,
            TemplateBodyUri = newUri,
            UpdatedAt = DateTime.UtcNow
        };

        await _customEmailTemplatesRepository.AddAsync(templateEntity);
    }
}