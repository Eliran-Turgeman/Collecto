using EmailCollector.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace EmailCollector.Api.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly EmailCollectorApiContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(EmailCollectorApiContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Update(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task Remove(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Upsert(T entity)
    {
        // Determine the primary key
        var key = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties
            .Select(p => p.Name).FirstOrDefault();

        if (key == null)
            throw new InvalidOperationException("Entity does not have a primary key.");

        var keyValue = typeof(T).GetProperty(key)?.GetValue(entity);

        if (keyValue == null)
            throw new InvalidOperationException("Entity primary key value is null.");

        var existingEntity = await GetByIdAsync(keyValue);

        if (existingEntity == null)
        {
            await AddAsync(entity);
        }
        else
        {
            _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
        }
    }
}
