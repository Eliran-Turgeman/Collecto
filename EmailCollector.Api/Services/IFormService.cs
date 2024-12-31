using EmailCollector.Api.DTOs;
using EmailCollector.Api.Services.Exports;

namespace EmailCollector.Api.Services;

/// <summary>
/// Defines methods for managing signup forms.
/// </summary>
public interface IFormService
{
    Task<FormDetailsDto> CreateFormAsync(Guid userId, ExtendedCreateFormDto extendedCreateFormDto);

    Task<IEnumerable<FormDto>> GetFormsByUserAsync(Guid userId);

    Task<FormDetailsDto?> GetFormByIdAsync(Guid formId, Guid userId);

    Task DeleteFormByIdAsync(Guid formId);

    Task<FormDetailsDto?> UpdateFormAsync(Guid formId, Guid userId, ExtendedCreateFormDto createFormDto);

    Task<IEnumerable<FormSummaryDetailsDto>> GetFormsSummaryDetailsAsync(Guid userId);

    Task<byte[]> ExportFormsAsync(IEnumerable<Guid> formIds, ExportFormat format);
}
