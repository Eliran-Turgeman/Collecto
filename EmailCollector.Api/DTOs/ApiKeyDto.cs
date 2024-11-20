namespace EmailCollector.Api.DTOs;

public class ApiKeyDto
{
    public required string Name { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? Expiration { get; set; } = null;
    public required bool IsRevoked { get; set; }
}