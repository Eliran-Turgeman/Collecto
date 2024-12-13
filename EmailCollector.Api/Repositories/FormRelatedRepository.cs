using EmailCollector.Api.Data;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;

namespace EmailCollector.Api.Repositories;

public class FormRelatedRepository<T> : Repository<T>, IFormRelatedRepository<T> where T : class
{
    private readonly EmailCollectorApiContext _context;
    private readonly DbSet<T> _dbSet;
    
    public FormRelatedRepository(EmailCollectorApiContext context) : base(context)
    {
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetByFormId(Guid formId)
    {
        ArgumentNullException.ThrowIfNull(formId);
        
        return await _dbSet.
            Where(entity => EF.Property<Guid>(entity, "FormId") == formId)
            .ToListAsync();
    }

    public async Task<Dictionary<Guid, IEnumerable<T>>> GetByFormIds(IEnumerable<Guid> formIds)
    {
        ArgumentNullException.ThrowIfNull(formIds, nameof(formIds));

        var formIdsList = formIds.ToList();

        if (!formIdsList.Any())
            return new Dictionary<Guid, IEnumerable<T>>();

        // Fetch and group entities by FormId directly
        var groupedEntities = await _dbSet
            .Where(entity => formIdsList.Contains(EF.Property<Guid>(entity, "FormId")))
            .GroupBy(entity => EF.Property<Guid>(entity, "FormId"))
            .ToDictionaryAsync(
                group => group.Key,
                group => (IEnumerable<T>)group.AsEnumerable()
            );

        // Ensure all formIds are present in the dictionary
        foreach (var formId in formIdsList)
        {
            if (!groupedEntities.ContainsKey(formId))
            {
                groupedEntities[formId] = Enumerable.Empty<T>();
            }
        }

        return groupedEntities;
    }
}