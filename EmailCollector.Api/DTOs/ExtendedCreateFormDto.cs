namespace EmailCollector.Api.DTOs;

public class ExtendedCreateFormDto : CreateFormDto
{
    public string? EmailFrom { get; set; }
    public string? SmtpServer { get; set; }
    public int? SmtpPort { get; set; }
    public string? SmtpUsername { get; set; }
    public string? SmtpPassword { get; set; }
    public string? AllowedOrigins { get; set; }
    public string? CaptchaSiteKey { get; set; }
    public string? CaptchaSecretKey { get; set; }
}