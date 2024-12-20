using EmailCollector.Api.DTOs;
using EmailCollector.Api.Repositories;
using EmailCollector.Api.Services.CustomEmailTemplates.StorageProviders;
using EmailCollector.Domain.Entities;
using EmailCollector.Domain.Enums;

namespace EmailCollector.Api.Services.CustomEmailTemplates;

/// <summary>
/// Service for managing custom email templates, including CRUD operations and template storage.
/// </summary>
public class CustomEmailTemplatesService : ICustomEmailTemplatesService
{
    private IFormRelatedRepository<CustomEmailTemplate> _customEmailTemplatesRepository;
    private readonly ITemplateStorageProvider _templateStorageProvider;

    /// <summary>
    /// Initializes a new instance of the CustomEmailTemplatesService class.
    /// </summary>
    /// <param name="customEmailTemplatesRepository">Repository for managing custom email templates</param>
    /// <param name="templateStorageProvider">Provider for template storage operations</param>
    public CustomEmailTemplatesService(IFormRelatedRepository<CustomEmailTemplate> customEmailTemplatesRepository,
        ITemplateStorageProvider templateStorageProvider)
    {
        _customEmailTemplatesRepository = customEmailTemplatesRepository;
        _templateStorageProvider = templateStorageProvider;
    }
    
    /// <summary>
    /// Retrieves all custom email templates associated with a specific form.
    /// </summary>
    /// <param name="formId">The ID of the form to get templates for</param>
    /// <returns>A collection of custom email templates</returns>
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

    /// <summary>
    /// Retrieves custom email templates for multiple forms, organized by form ID.
    /// </summary>
    /// <param name="formIds">Collection of form IDs to get templates for</param>
    /// <returns>Dictionary mapping form IDs to their associated templates</returns>
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

    /// <summary>
    /// Retrieves a specific email template for a form and event type.
    /// </summary>
    /// <param name="formId">The ID of the form</param>
    /// <param name="templateEvent">The event type of the template</param>
    /// <returns>The matching email template, or null if not found</returns>
    public async Task<CustomEmailTemplateDto?> GetCustomEmailTemplateByFormIdAndEvent(Guid formId, TemplateEvent templateEvent)
    {
        var templatesForForm = await GetCustomEmailTemplatesByFormId(formId);
        return templatesForForm
            .FirstOrDefault(t => t.Event == templateEvent);
    }

    /// <summary>
    /// Creates or updates a custom email template.
    /// </summary>
    /// <param name="customEmailTemplateDto">The template data to save</param>
    /// <exception cref="ArgumentNullException">Thrown when customEmailTemplateDto is null</exception>
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

    /// <summary>
    /// Deletes a custom email template and its associated template body.
    /// </summary>
    /// <param name="templateId">The ID of the template to delete</param>
    public async Task DeleteCustomEmailTemplate(Guid templateId)
    {
        await _customEmailTemplatesRepository.RemoveById(templateId);
        _templateStorageProvider.DeleteTemplateBodyAsync(templateId.ToString());
    }

    /// <summary>
    /// Retrieves a custom email template by its ID.
    /// </summary>
    /// <param name="id">The ID of the template to retrieve</param>
    /// <returns>The email template with the specified ID</returns>
    /// <exception cref="KeyNotFoundException">Thrown when no template exists with the specified ID</exception>
    public async Task<CustomEmailTemplateDto> GetCustomEmailTemplateById(Guid id)
    {
        var entity = await _customEmailTemplatesRepository.GetByIdAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"Template with ID {id} not found");

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