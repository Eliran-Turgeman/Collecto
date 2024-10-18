namespace EmailCollector.Domain.Entities;

public class FormCorsSettings
{
    public int FormId { get; set; }
    public string AllowedOrigins { get; set; }
    public SignupForm Form { get; set; }
}
