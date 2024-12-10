using MediatR;

namespace EmailCollector.Domain.Notifications;

public class EmailAddedSuccessfullyNotification : INotification
{
    public string EmailAddress { get; set; }
    public Guid SignupFormId { get; set; }
}