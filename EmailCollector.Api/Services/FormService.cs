using EmailCollector.Api.DTOs;
using EmailCollector.Domain.Entities;
using EmailCollector.Api.Repositories;

namespace EmailCollector.Api.Services;

/// <summary>
/// Implements form management functionality.
/// </summary>
public class FormService : IFormService
{
    private readonly ISignupFormRepository _signupFormRepository;
    private readonly ILogger<FormService> _logger;

    public FormService(ISignupFormRepository signupFormRepository,
        ILogger<FormService> logger)
    {
        _signupFormRepository = signupFormRepository;
        _logger = logger;
    }

    public async Task<FormDetailsDto> CreateFormAsync(Guid userId, CreateFormDto createFormDto)
    {
        var signupForm = new SignupForm
        {
            FormName = createFormDto.FormName,
            CreatedBy = userId,
        };

        await _signupFormRepository.AddAsync(signupForm);

        _logger.LogInformation($"Created form {signupForm.Id} for user {userId}.");

        return new FormDetailsDto
        {
            Id = signupForm.Id,
            FormName = signupForm.FormName,
            Status = signupForm.Status,
        };
    }

    public async Task<IEnumerable<FormDto>> GetFormsByUserAsync(Guid userId)
    {
        var forms = await _signupFormRepository.GetByUserIdAsync(userId);

        _logger.LogInformation($"Found {forms.Count()} forms for user {userId}.");

        return forms.Select(form => new FormDto
        {
            Id = form.Id,
            FormName = form.FormName,
        });
    }

    public async Task<FormDetailsDto?> GetFormByIdAsync(int formId, Guid userId)
    {
        var form = await _signupFormRepository.GetByFormIdentifierAsync(formId, userId);

        if (form == null || form.CreatedBy != userId)
        {
            _logger.LogInformation($"Form {formId} not found or user is not the creator.");
            return null;
        }

        _logger.LogInformation($"Found form {formId}.");

        return new FormDetailsDto
        {
            Id = form.Id,
            FormName = form.FormName,
            Status = form.Status,
        };
    }

    public async Task DeleteFormByIdAsync(int formId, Guid userId)
    {
        var form = await _signupFormRepository.GetByIdAsync(formId);

        if (form == null || form.CreatedBy != userId)
        {
            _logger.LogInformation($"Form {formId} not found or user is not the creator.");
            return;
        }

        await _signupFormRepository.Remove(form);
    }

    public async Task<FormDetailsDto?> UpdateFormAsync(int formId, Guid userId, CreateFormDto createFormDto)
    {
        var form = await _signupFormRepository.GetByFormIdentifierAsync(formId, userId);

        if (form == null || form.CreatedBy != userId)
        {
            _logger.LogInformation($"Form {formId} not found or user is not the creator.");
            return null;
        }

        form.FormName = createFormDto.FormName;
        form.Status = createFormDto.Status;

        await _signupFormRepository.Update(form);

        _logger.LogInformation($"Updated form {formId}.");

        return new FormDetailsDto
        {
            Id = form.Id,
            FormName = form.FormName,
            Status = form.Status,
        };
    }
}