using System.Linq.Expressions;
using TaskNow.DAO.Entities;
using TaskNow.DTO.Entities;

namespace TaskNow.DAL.Entities.Interfaces;

public interface IListaDAL
{
    Task<ListaDTO?> GetByIdAsync(int id);
    Task<ListaDTO> CreateAsync(ListaDTO dto);
    Task<ListaDTO?> EditAsync(int id, ListaDTO dto);
    Task<bool> DeleteAsync(int id);
    IQueryable<Lista> GetQuery(bool asNoTracking, params Expression<Func<Lista, object>>[] includes);
    Task<List<ListaDTO>> ObterPorQuadroOrdenadoAsync(int quadroId);
    Task<int> ObterProximaOrdemAsync(int quadroId);
}