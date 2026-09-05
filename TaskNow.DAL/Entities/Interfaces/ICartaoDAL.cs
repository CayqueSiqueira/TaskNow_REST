using TaskNow.DAL.Base;
using TaskNow.DAO.Entities;
using TaskNow.DTO.Entities;

namespace TaskNow.DAL.Entities.Interfaces;

public interface ICartaoDAL : IBaseDAL<Cartao, CartaoDTO>
{
    Task<List<CartaoDTO>> ObterPorListaOrdenadoAsync(int listaId);
    Task<int> ObterProximaOrdemAsync(int listaId);
    Task<bool> CartaoPossuiEtiquetaAsync(int cartaoId, int etiquetaId);
    Task<bool> AssociarEtiquetaAsync(int cartaoId, int etiquetaId);
    Task<bool> RemoverEtiquetaAsync(int cartaoId, int etiquetaId);
}
