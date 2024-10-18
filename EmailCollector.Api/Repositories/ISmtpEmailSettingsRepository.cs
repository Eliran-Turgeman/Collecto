using EmailCollector.Domain.Entities;
using EmailCollector.Domain.Interfaces.Repositories;

namespace EmailCollector.Api.Repositories;

public interface ISmtpEmailSettingsRepository : IRepository<SmtpEmailSettings>
{
}
