using EmailCollector.Api.DTOs;
using EmailCollector.Api.Services.Exports;

namespace EmailCollector.Api.Services;

/// <summary>
/// Defines methods for managing signup forms.
/// </summary>
public interface IFormService
{
    Task<FormDetailsDto> CreateFormAsync(Guid userId, CreateFormDto createFormDto);
    Task<FormDetailsDto> CreateFormAsync(Guid userId, ExtendedCreateFormDto extendedCreateFormDto);

    Task<IEnumerable<FormDto>> GetFormsByUserAsync(Guid userId);

    Task<FormDetailsDto?> GetFormByIdAsync(int formId, Guid userId);

    Task DeleteFormByIdAsync(int formId);

    Task<FormDetailsDto?> UpdateFormAsync(int formId, Guid userId, CreateFormDto createFormDto);

    Task<IEnumerable<FormSummaryDetailsDto>> GetFormsSummaryDetailsAsync(Guid userId);

    Task<byte[]> ExportFormsAsync(IEnumerable<int> formIds, ExportFormat format);
}
