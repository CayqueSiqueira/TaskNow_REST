using System.Linq.Expressions;
using TaskNow.DAO.Entities;
using TaskNow.DTO.Entities;

namespace TaskNow.DAL.Entities.Interfaces;

public interface ICartaoDAL
{
    Task<CartaoDTO?> GetByIdAsync(int id);
    Task<CartaoDTO> CreateAsync(CartaoDTO dto);
    Task<CartaoDTO?> EditAsync(int id, CartaoDTO dto);
    Task<bool> DeleteAsync(int id);
    IQueryable<Cartao> GetQuery(bool asNoTracking, params Expression<Func<Cartao, object>>[] includes);
    Task<List<CartaoDTO>> ObterPorListaOrdenadoAsync(int listaId);
    Task<int> ObterProximaOrdemAsync(int listaId);
}