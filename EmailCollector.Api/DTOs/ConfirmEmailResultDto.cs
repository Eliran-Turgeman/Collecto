using EmailCollector.Domain.Enums;

namespace EmailCollector.Api.DTOs;

public class ConfirmEmailResultDto
{
    public bool Success { get; set; }

    public required string Message { get; set; }

    public EmailConfirmationErrorCode? ErrorCode { get; set; }
}
