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
}
