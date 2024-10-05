namespace EmailCollector.Api.DTOs;

/// <summary>
/// DTO for displaying detailed form information.
/// </summary>
public class FormDetailsDto
{
    public int Id { get; set; }

    public required string FormName { get; set; }

}