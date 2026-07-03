using TaskNow.DTO.Entities;
using TaskNow.DTO.Requests.Quadro;
using TaskNow.DTO.Utils;

namespace TaskNow.BLL.Entities.Interfaces;

public interface IQuadroBLL
{
    Task<RetornoDTO<List<QuadroDTO>>> ListarAsync();
    Task<RetornoDTO<QuadroDTO>> ObterAsync(int id);
    Task<RetornoDTO<QuadroDTO>> CriarAsync(QuadroCriarRequestDTO request);
    Task<RetornoDTO<QuadroDTO>> EditarAsync(int id, QuadroEditarRequestDTO request);
    Task<RetornoDTO<bool>> ExcluirAsync(int id);
}