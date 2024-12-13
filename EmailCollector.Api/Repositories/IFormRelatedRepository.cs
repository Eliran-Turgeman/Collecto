namespace EmailCollector.Api.Repositories;

public interface IFormRelatedRepository<T> : IRepository<T> where T : class 
{
    Task<IEnumerable<T>> GetByFormId(Guid formId);
    Task<Dictionary<Guid, IEnumerable<T>>> GetByFormIds(IEnumerable<Guid> formIds);
}