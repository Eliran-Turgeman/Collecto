namespace EmailCollector.Api.Repositories;

public interface IFormRelatedRepository<T> : IRepository<T> where T : class 
{
    Task<IEnumerable<T>> GetByFormId(Guid formId);
}