using System.Linq.Expressions;

namespace TaskNow.DAL.Base;

public interface IBaseDAL<TEntity, TDTO>
    where TEntity : class
    where TDTO : class
{
    Task<TDTO?> GetByIdAsync(int id);
    Task<List<TDTO>> GetAllAsync();
    Task<(List<TDTO> Itens, int Total)> GetPagedAsync(
        int pagina,
        int tamanhoPagina,
        IQueryable<TEntity>? query = null);
    Task<TDTO> CreateAsync(TDTO dto);
    Task<TDTO?> EditAsync(int id, TDTO dto);
    Task<bool> DeleteAsync(int id);
    IQueryable<TEntity> GetQuery(bool asNoTracking, params Expression<Func<TEntity, object>>[] includes);
}
