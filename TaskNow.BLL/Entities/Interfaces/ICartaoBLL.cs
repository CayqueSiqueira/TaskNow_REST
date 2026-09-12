using TaskNow.DTO.Entities;
using TaskNow.DTO.Requests.Cartao;
using TaskNow.DTO.Utils;

namespace TaskNow.BLL.Entities.Interfaces;

public interface ICartaoBLL
{
    Task<RetornoDTO<CartaoDTO>> ObterPorIdAsync(int id);
    Task<RetornoDTO<CartaoDTO>> CriarAsync(CartaoCriarRequestDTO request);
    Task<RetornoDTO<CartaoDTO>> EditarAsync(int id, CartaoEditarRequestDTO request);
    Task<RetornoDTO<bool>> MoverAsync(int cartaoId, CartaoMoverRequestDTO request);
    Task<RetornoDTO<bool>> ExcluirAsync(int id);
    Task<RetornoDTO<bool>> AssociarEtiquetaAsync(int cartaoId, int etiquetaId);
    Task<RetornoDTO<bool>> RemoverEtiquetaAsync(int cartaoId, int etiquetaId);
}