namespace EmailCollector.Domain.Entities;

public class RecaptchaFormSettings
{
    public int FormId { get; set; }
    public string? SiteKey { get; set; }
    public string? SecretKey { get; set; }
    public SignupForm Form { get; set; }
}