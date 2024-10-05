using EmailCollector.Application.DTOs;
using EmailCollector.Application.Interfaces;
using EmailCollector.Domain.Entities;
using EmailCollector.Domain.Interfaces.Repositories;

namespace EmailCollector.Application.Services;

/// <summary>
/// Implements form management functionality.
/// </summary>
public class FormService : IFormService
{
    private readonly ISignupFormRepository _signupFormRepository;

    public FormService(ISignupFormRepository signupFormRepository)
    {
        _signupFormRepository = signupFormRepository;
    }

    public async Task<FormDetailsDto> CreateFormAsync(string userId, CreateFormDto createFormDto)
    {
        if (!int.TryParse(userId, out var parsedUserId))
        {
            throw new ApplicationException("Invalid user ID format.");
        }

        // Validate allowed domains
        var allowedDomains = new List<string>();
        foreach (var domain in createFormDto.AllowedDomains)
        {
            try
            {
                // validate domain format here
                // ....
                allowedDomains.Add(domain);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Invalid domain '{domain}': {ex.Message}");
            }
        }

        // Create the SignupForm entity
        var signupForm = new SignupForm
        {
            FormName = createFormDto.FormName,
            CreatedBy = parsedUserId,
        };

        await _signupFormRepository.AddAsync(signupForm);

        // Map to FormDetailsDto
        var formDetailsDto = new FormDetailsDto
        {
            Id = signupForm.Id,
            FormName = signupForm.FormName,
        };

        return formDetailsDto;
    }

    public async Task<IEnumerable<FormDto>> GetFormsByUserAsync(string userId)
    {
        var forms = await _signupFormRepository.GetByUserIdAsync(userId);

        // Map to FormDto
        var formDtos = forms.Select(form => new FormDto
        {
            Id = form.Id,
            FormName = form.FormName,
        });

        return formDtos;
    }

    public async Task<FormDetailsDto> GetFormByIdAsync(int formId, string userId)
    {
        if (!int.TryParse(userId, out var parsedUserId))
        {
            throw new ApplicationException("Invalid user ID format.");
        }

        var form = await _signupFormRepository.GetByIdAsync(formId);

        if (form == null || form.CreatedBy != parsedUserId)
        {
            throw new ApplicationException("Form not found or access denied.");
        }

        // Map to FormDetailsDto
        var formDetailsDto = new FormDetailsDto
        {
            Id = form.Id,
            FormName = form.FormName,
        };

        return formDetailsDto;
    }
}