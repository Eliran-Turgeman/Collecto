using System.ComponentModel.DataAnnotations;

namespace EmailCollector.Application.DTOs;

/// <summary>
/// DTO for creating a new signup form.
/// </summary>
public class CreateFormDto
{
    [Required]
    public required string FormName { get; set; }

    [Required]
    public required List<string> AllowedDomains { get; set; }
}