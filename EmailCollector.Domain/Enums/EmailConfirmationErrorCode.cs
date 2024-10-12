namespace EmailCollector.Domain.Enums;

public enum EmailConfirmationErrorCode
{
    ExpiredToken,
    InvalidToken,
    EmailAlreadyConfirmed,
}
