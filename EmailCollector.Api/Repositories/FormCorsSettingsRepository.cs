using EmailCollector.Api.Data;
using EmailCollector.Domain.Entities;

namespace EmailCollector.Api.Repositories;

public class FormCorsSettingsRepository : IFormCorsSettingsRepository
{
    private readonly EmailCollectorApiContext _dbContext;
    private readonly ILogger<FormCorsSettingsRepository> _logger;

    public FormCorsSettingsRepository(EmailCollectorApiContext dbContenxt, ILogger<FormCorsSettingsRepository> logger)
    {
        _dbContext = dbContenxt;
        _logger = logger;
    }

    public async Task AddAsync(FormCorsSettings entity)
    {
        _dbContext.FormCorsSettings.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<FormCorsSettings?> GetByIdAsync(object id)
    {
        return await _dbContext.FormCorsSettings.FindAsync(id);
    }

    public async Task Remove(FormCorsSettings entity)
    {
        _dbContext.FormCorsSettings.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(FormCorsSettings entity)
    {
        _dbContext.FormCorsSettings.Update(entity);
        await _dbContext.SaveChangesAsync();
    }
}
