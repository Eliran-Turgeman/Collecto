namespace EmailCollector.Domain.Entities;

/// <summary>
/// Represents an email collected from a signup form.
/// </summary>
public class EmailSignup
{
    public Guid Id { get; set; }

    public required string EmailAddress { get; set; }

    public Guid SignupFormId { get; set; }

    public DateTime SignupDate { get; set; } = DateTime.UtcNow;
}
