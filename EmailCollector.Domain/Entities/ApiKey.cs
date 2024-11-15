namespace EmailCollector.Domain.Entities;

public class ApiKey
{
    public Guid Id { get; set; }
    public string KeyHash { get; set; }
    public string UserId { get; set; } // FK to EmailCollectorApiUser
    public EmailCollectorApiUser User { get; set; } // navigation property
    public DateTime CreatedAt { get; set; }
    public DateTime? Expiration { get; set; }
    public string Name { get; set; }
    public bool IsRevoked { get; set; }
}
