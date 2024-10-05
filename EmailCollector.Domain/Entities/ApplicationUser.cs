namespace EmailCollector.Domain.Entities;

/// <summary>
/// Represents a user in the system.
/// </summary>
public class ApplicationUser
{
    public int Id { get; set; }

    public required string UserName { get; set; }

    public required string Email { get; set; }
}
