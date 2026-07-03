using TaskNow.DAL.Base;
using TaskNow.DAO.Entities;
using TaskNow.DTO.Entities;

namespace TaskNow.DAL.Entities.Interfaces;

public interface IMembroQuadroDAL : IBaseDAL<MembroQuadro, MembroQuadroDTO>
{
    Task<bool> EhMembroAsync(int quadroId, string usuarioId);
    Task<List<MembroQuadroDTO>> ObterPorQuadroAsync(int quadroId);
}
