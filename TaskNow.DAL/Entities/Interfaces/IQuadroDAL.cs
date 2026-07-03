using System.Linq.Expressions;
using TaskNow.DAO.Entities;
using TaskNow.DTO.Entities;

namespace TaskNow.DAL.Entities.Interfaces;

public interface IQuadroDAL
{
    Task<QuadroDTO?> GetByIdAsync(int id);
    Task<QuadroDTO> CreateAsync(QuadroDTO dto);
    Task<QuadroDTO?> EditAsync(int id, QuadroDTO dto);
    Task<bool> DeleteAsync(int id);
    IQueryable<Quadro> GetQuery(bool asNoTracking, params Expression<Func<Quadro, object>>[] includes);
    Task<List<QuadroDTO>> ObterPorUsuarioAsync(string usuarioId);
    Task<bool> UsuarioTemAcessoAsync(int quadroId, string usuarioId);
}