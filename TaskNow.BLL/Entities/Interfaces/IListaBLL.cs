using TaskNow.DTO.Entities;
using TaskNow.DTO.Requests.Lista;
using TaskNow.DTO.Utils;

namespace TaskNow.BLL.Entities.Interfaces;

public interface IListaBLL
{
    Task<RetornoDTO<List<ListaDTO>>> ListarPorQuadroAsync(int quadroId);
    Task<RetornoDTO<ListaDTO>> CriarAsync(ListaCriarRequestDTO request);
    Task<RetornoDTO<ListaDTO>> EditarAsync(int id, ListaCriarRequestDTO request);
    Task<RetornoDTO<bool>> ExcluirAsync(int id);
    Task<RetornoDTO<List<ListaDTO>>> CriarPadraoAsync(int quadroId);
    Task<RetornoDTO<List<ListaDTO>>> ReordenarAsync(int quadroId, ListaReordenarRequestDTO request);

}