using EmailCollector.Api.Services.Exports;

namespace EmailCollector.Api.DTOs;

/// <summary>
/// Contains details about a form, including the number of submissions and the date it was created.
/// </summary>
public class FormSummaryDetailsDto : IExportable
{
    /// <summary>
    /// The unique identifier for the form.
    /// </summary>
    [ExportField("Form ID")]
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the form.
    /// </summary>
    [ExportField("Form Name")]
    public required string FormName { get; set; }

    /// <summary>
    /// The number of submissions for the form.
    /// </summary>
    [ExportField("Total Submissions")]
    public int SubmissionsCount { get; set; }

    /// <summary>
    /// The date the form was created.
    /// </summary>
    [ExportField("Creation Date")]
    public DateTime CreatedAt { get; set; }
}
