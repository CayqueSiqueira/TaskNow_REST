using AutoMapper;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskNow.DAO;

namespace TaskNow.DAL.Base;

public abstract class BaseDAL<TEntity, TDTO>
    where TEntity : class
    where TDTO : class
{
    protected readonly TaskNowDbContext _context;
    protected readonly IMapper _mapper;
    protected readonly DbSet<TEntity> _dbSet;

    protected BaseDAL(TaskNowDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TDTO?> GetByIdAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        return entity is null ? null : _mapper.Map<TDTO>(entity);
    }

    public virtual async Task<List<TDTO>> GetAllAsync()
    {
        var entities = await _dbSet.AsNoTracking().ToListAsync();
        return _mapper.Map<List<TDTO>>(entities);
    }

    public virtual async Task<(List<TDTO> Itens, int Total)> GetPagedAsync(
        int pagina,
        int tamanhoPagina,
        IQueryable<TEntity>? query = null)
    {
        query ??= _dbSet.AsNoTracking();

        var total = await query.CountAsync();
        var skip = (pagina - 1) * tamanhoPagina;

        var entities = await query
            .Skip(skip)
            .Take(tamanhoPagina)
            .ToListAsync();

        return (_mapper.Map<List<TDTO>>(entities), total);
    }

    public virtual async Task<TDTO> CreateAsync(TDTO dto)
    {
        var entity = _mapper.Map<TEntity>(dto);
        _dbSet.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.Map<TDTO>(entity);
    }

    public virtual async Task<TDTO?> EditAsync(int id, TDTO dto)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity is null)
        {
            return null;
        }

        _mapper.Map(dto, entity);
        await _context.SaveChangesAsync();
        return _mapper.Map<TDTO>(entity);
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity is null)
        {
            return false;
        }

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public IQueryable<TEntity> GetQuery(bool asNoTracking, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return asNoTracking ? query.AsNoTracking() : query;
    }
}
