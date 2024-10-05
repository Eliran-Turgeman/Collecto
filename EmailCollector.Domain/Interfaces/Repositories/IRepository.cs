namespace EmailCollector.Domain.Interfaces.Repositories;

/// <summary>
/// Generic repository interface for CRUD operations.
/// </summary>
public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(object id);

    Task<IEnumerable<T>> GetAllAsync();

    Task AddAsync(T entity);

    void Update(T entity);

    void Remove(T entity);
}
