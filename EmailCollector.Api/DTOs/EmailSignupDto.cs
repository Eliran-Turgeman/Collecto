using System.ComponentModel.DataAnnotations;

namespace EmailCollector.Api.DTOs;

/// <summary>
/// DTO for submitting an email signup.
/// </summary>
public class EmailSignupDto
{
    [Required]
    public Guid FormId { get; set; }

    [Required]
    public required string Email { get; set; }

    public DateTime? SignupDate { get; set; }
    
    public string? RecaptchaToken { get; set; }
}