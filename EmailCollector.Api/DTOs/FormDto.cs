namespace EmailCollector.Api.DTOs;

/// <summary>
/// DTO for displaying basic form information.
/// </summary>
public class FormDto
{
    public Guid Id { get; set; }

    public required string FormName { get; set; }
}