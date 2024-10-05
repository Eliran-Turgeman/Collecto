namespace EmailCollector.Domain.Entities;

/// <summary>
/// Represents an email collected from a signup form.
/// </summary>
public class EmailSignup
{
    public int Id { get; set; }

    public required string EmailAddress { get; set; }

    public int SignupFormId { get; set; }

    public DateTime SignupDate { get; set; } = DateTime.UtcNow;
}
