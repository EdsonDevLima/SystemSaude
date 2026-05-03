using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SystemSaude.Domain.Interfaces;
using SystemSaude.Infrastructure.Data;

namespace SystemSaude.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public void Remove(T entity)
    {
        DbSet.Remove(entity);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Context.SaveChangesAsync(cancellationToken);
    }
}
