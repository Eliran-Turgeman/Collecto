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

        // Check if the entity already exists
        var existingEntity = await _customEmailTemplatesRepository.GetByIdAsync(customEmailTemplateDto.Id);
        if (existingEntity != null)
        {
            // Update existing entity
            existingEntity.Event = customEmailTemplateDto.Event;
            existingEntity.TemplateSubject = customEmailTemplateDto.TemplateSubject;
            existingEntity.TemplateBodyUri = await _templateStorageProvider.SaveTemplateBodyAsync(
                customEmailTemplateDto.TemplateBody, 
                customEmailTemplateDto.TemplateBodyUri
            );
            existingEntity.IsActive = customEmailTemplateDto.IsActive;
            existingEntity.UpdatedAt = DateTime.UtcNow;

            await _customEmailTemplatesRepository.Update(existingEntity);
        }
        else
        {
            // Create new entity
            var newEntity = new CustomEmailTemplate
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Event = customEmailTemplateDto.Event,
                FormId = customEmailTemplateDto.FormId,
                IsActive = customEmailTemplateDto.IsActive,
                TemplateSubject = customEmailTemplateDto.TemplateSubject,
                TemplateBodyUri = await _templateStorageProvider.SaveTemplateBodyAsync(
                    customEmailTemplateDto.TemplateBody, 
                    customEmailTemplateDto.TemplateBodyUri
                ),
                UpdatedAt = DateTime.UtcNow
            };
        
            await _customEmailTemplatesRepository.AddAsync(newEntity);
        }
    }

    public async Task DeleteCustomEmailTemplate(Guid templateId)
    {
        await _customEmailTemplatesRepository.RemoveById(templateId);
        _templateStorageProvider.DeleteTemplateBodyAsync(templateId.ToString());
    }

    public async Task<CustomEmailTemplateDto> GetCustomEmailTemplateById(Guid id)
    {
        var entity = await _customEmailTemplatesRepository.GetByIdAsync(id);
        var templateBody = await _templateStorageProvider.GetTemplateBodyAsync(entity.TemplateBodyUri);

        return new CustomEmailTemplateDto
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            Event = entity.Event,
            FormId = entity.FormId,
            IsActive = entity.IsActive,
            TemplateSubject = entity.TemplateSubject,
            TemplateBodyUri = entity.TemplateBodyUri,
            TemplateBody = templateBody,
            UpdatedAt = entity.UpdatedAt
        };
    }
}