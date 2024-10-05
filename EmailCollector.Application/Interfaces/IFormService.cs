using EmailCollector.Application.DTOs;

namespace EmailCollector.Application.Interfaces;

/// <summary>
/// Defines methods for managing signup forms.
/// </summary>
public interface IFormService
{
    Task<FormDetailsDto> CreateFormAsync(string userId, CreateFormDto createFormDto);

    Task<IEnumerable<FormDto>> GetFormsByUserAsync(string userId);

    Task<FormDetailsDto> GetFormByIdAsync(int formId, string userId);
}
