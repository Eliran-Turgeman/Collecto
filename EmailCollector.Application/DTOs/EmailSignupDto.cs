using System.ComponentModel.DataAnnotations;

namespace EmailCollector.Application.DTOs;

/// <summary>
/// DTO for submitting an email signup.
/// </summary>
public class EmailSignupDto
{
    [Required]
    public int FormId { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    /// <summary>
    /// The domain from which the request originated
    /// </summary>
    public required string Origin { get; set; }
}