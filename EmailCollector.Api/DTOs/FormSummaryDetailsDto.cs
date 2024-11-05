namespace EmailCollector.Api.DTOs;

/// <summary>
/// Contains details about a form, including the number of submissions and the date it was created.
/// </summary>
public class FormSummaryDetailsDto
{
    /// <summary>
    /// The unique identifier for the form.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the form.
    /// </summary>
    public required string FormName { get; set; }

    /// <summary>
    /// The number of submissions for the form.
    /// </summary>
    public int SubmissionsCount { get; set; }

    /// <summary>
    /// The date the form was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
