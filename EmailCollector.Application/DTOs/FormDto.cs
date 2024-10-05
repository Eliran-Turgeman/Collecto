namespace EmailCollector.Application.DTOs;

/// <summary>
/// DTO for displaying basic form information.
/// </summary>
public class FormDto
{
    public int Id { get; set; }

    public required string FormName { get; set; }
}