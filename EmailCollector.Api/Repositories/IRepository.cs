namespace EmailCollector.Api.Repositories;

/// <summary>
/// Generic repository interface for CRUD operations.
/// </summary>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(object id);

    Task AddAsync(T entity);

    Task Update(T entity);

    Task Remove(T entity);
    
    Task Upsert(T entity);
    
    Task RemoveById(object id);
}
