using EmailCollector.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace EmailCollector.Api.DTOs;

/// <summary>
/// DTO for creating a new signup form.
/// </summary>
public class CreateFormDto
{
    [Required]
    public required string FormName { get; set; }

    public FormStatus Status { get; set; } = FormStatus.Active;
}