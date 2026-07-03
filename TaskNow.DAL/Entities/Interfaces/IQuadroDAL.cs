using TaskNow.DAL.Base;
using TaskNow.DAO.Entities;
using TaskNow.DTO.Entities;

namespace TaskNow.DAL.Entities.Interfaces;

public interface IQuadroDAL : IBaseDAL<Quadro, QuadroDTO>
{
    Task<List<QuadroDTO>> ObterPorUsuarioAsync(string usuarioId);
    Task<bool> UsuarioTemAcessoAsync(int quadroId, string usuarioId);
}
