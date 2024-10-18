using EmailCollector.Api.Data;
using EmailCollector.Domain.Entities;

namespace EmailCollector.Api.Repositories;

public class SmtpEmailSettingsRepository : ISmtpEmailSettingsRepository
{
    private readonly EmailCollectorApiContext _dbContext;
    private readonly ILogger<SmtpEmailSettingsRepository> _logger;

    public SmtpEmailSettingsRepository(EmailCollectorApiContext dbContenxt, ILogger<SmtpEmailSettingsRepository> logger)
    {
        _dbContext = dbContenxt;
        _logger = logger;
    }

    public async Task AddAsync(SmtpEmailSettings entity)
    {
        _dbContext.SmtpEmailSettings.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<SmtpEmailSettings?> GetByIdAsync(object id)
    {
        return await _dbContext.SmtpEmailSettings.FindAsync(id);
    }

    public async Task Remove(SmtpEmailSettings entity)
    {
        _dbContext.SmtpEmailSettings.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(SmtpEmailSettings entity)
    {
        _dbContext.SmtpEmailSettings.Update(entity);
        await _dbContext.SaveChangesAsync();
    }
}
