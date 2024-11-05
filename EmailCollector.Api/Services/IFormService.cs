using EmailCollector.Api.DTOs;

namespace EmailCollector.Api.Services;

/// <summary>
/// Defines methods for managing signup forms.
/// </summary>
public interface IFormService
{
    Task<FormDetailsDto> CreateFormAsync(Guid userId, CreateFormDto createFormDto);

    Task<IEnumerable<FormDto>> GetFormsByUserAsync(Guid userId);

    Task<FormDetailsDto?> GetFormByIdAsync(int formId, Guid userId);

    Task DeleteFormByIdAsync(int formId, Guid userId);

    Task<FormDetailsDto?> UpdateFormAsync(int formId, Guid userId, CreateFormDto createFormDto);

    Task<IEnumerable<FormSummaryDetailsDto?>> GetFormsSummaryDetailsAsync(Guid userId);
}
