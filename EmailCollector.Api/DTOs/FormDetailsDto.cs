using EmailCollector.Domain.Enums;

namespace EmailCollector.Api.DTOs;

/// <summary>
/// DTO for displaying detailed form information.
/// </summary>
public class FormDetailsDto
{
    public Guid Id { get; set; }

    public required string FormName { get; set; }

    public FormStatus Status { get; set; }
}