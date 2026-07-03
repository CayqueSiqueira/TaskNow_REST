using TaskNow.DAL.Base;
using TaskNow.DAO.Entities;
using TaskNow.DTO.Entities;

namespace TaskNow.DAL.Entities.Interfaces;

public interface IComentarioDAL : IBaseDAL<Comentario, ComentarioDTO>
{
    Task<List<ComentarioDTO>> ObterPorCartaoAsync(int cartaoId);
}
