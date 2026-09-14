using TaskNow.DAL.Base;
using TaskNow.DAO.Entities;
using TaskNow.DTO.Entities;

namespace TaskNow.DAL.Entities.Interfaces;

public interface IMembroQuadroDAL : IBaseDAL<MembroQuadro, MembroQuadroDTO>
{
    Task<List<MembroQuadroDTO>> ObterPorQuadroAsync(int quadroId);
    Task<MembroQuadroDTO?> ObterPorUsuarioEQuadroAsync(int quadroId, string usuarioId);
    Task<bool> EhDonoAsync(int quadroId, string usuarioId);
    Task<bool> EhMembroAsync(int quadroId, string usuarioId);
    Task<bool> RemoverMembroAsync(int quadroId, string usuarioId);
}
