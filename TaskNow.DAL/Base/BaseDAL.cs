using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskNow.DAO;

namespace TaskNow.DAL.Base;

public abstract class BaseDAL<TEntity, TDTO>(TaskNowDbContext context)
    where TEntity : class
{
    protected TaskNowDbContext Context { get; } = context;
    protected DbSet<TEntity> DbSet => Context.Set<TEntity>();

    public IQueryable<TEntity> GetQuery(bool asNoTracking, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = DbSet;

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return query;
    }

    public abstract Task<TDTO?> GetByIdAsync(int id);
    public abstract Task<TDTO> CreateAsync(TDTO dto);
    public abstract Task<TDTO?> EditAsync(int id, TDTO dto);

    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity is null)
        {
            return false;
        }

        DbSet.Remove(entity);
        await Context.SaveChangesAsync();
        return true;
    }
}