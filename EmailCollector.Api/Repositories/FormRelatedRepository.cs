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
    }

    public async Task<IEnumerable<T>> GetByFormId(Guid formId)
    {
        return await _dbSet.Where(entity => EF.Property<Guid>(entity, "FormId") == formId).ToListAsync();
    }
}