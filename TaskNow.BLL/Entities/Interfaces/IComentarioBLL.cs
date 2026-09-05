using TaskNow.DTO.Entities;
using TaskNow.DTO.Requests.Comentario;
using TaskNow.DTO.Utils;

namespace TaskNow.BLL.Entities.Interfaces;

public interface IComentarioBLL
{
    Task<RetornoDTO<List<ComentarioDTO>>> ListarPorCartaoAsync(int cartaoId);
    Task<RetornoDTO<ComentarioDTO>> CriarAsync(ComentarioCriarRequestDTO request);
    Task<RetornoDTO<ComentarioDTO>> EditarAsync(int id, ComentarioEditarRequestDTO request);
    Task<RetornoDTO<bool>> ExcluirAsync(int id);
}