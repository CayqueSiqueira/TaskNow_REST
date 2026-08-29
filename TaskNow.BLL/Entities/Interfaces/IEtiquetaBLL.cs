using TaskNow.DTO.Entities;
using TaskNow.DTO.Requests.Etiqueta;
using TaskNow.DTO.Utils;

namespace TaskNow.BLL.Entities.Interfaces;

public interface IEtiquetaBLL
{
    Task<RetornoDTO<List<EtiquetaDTO>>> ListarPorQuadroAsync(int quadroId);
    Task<RetornoDTO<EtiquetaDTO>> CriarAsync(EtiquetaCriarRequestDTO request);
    Task<RetornoDTO<EtiquetaDTO>> EditarAsync(int id, EtiquetaCriarRequestDTO request);
    Task<RetornoDTO<bool>> ExcluirAsync(int id);
}