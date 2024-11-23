using EmailCollector.Domain.Enums;

namespace EmailCollector.Domain.Entities;

/// <summary>
/// Represents a signup form created by a user.
/// </summary>
public class SignupForm
{
    public int Id { get; set; }

    public required string FormName { get; set; }

    public required Guid CreatedBy { get; set; }

    public FormStatus Status { get; set; } = FormStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public FormCorsSettings FormCorsSettings { get; set; }

    public FormEmailSettings FormEmailSettings { get; set; }
    
    public RecaptchaFormSettings RecaptchaSettings { get; set; }
}
