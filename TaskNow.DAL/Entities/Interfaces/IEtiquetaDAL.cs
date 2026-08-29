using TaskNow.DAL.Base;
using TaskNow.DAO.Entities;
using TaskNow.DTO.Entities;

namespace TaskNow.DAL.Entities.Interfaces;

public interface IEtiquetaDAL : IBaseDAL<Etiqueta, EtiquetaDTO>
{
    Task<List<EtiquetaDTO>> ObterPorQuadroAsync(int quadroId);
    Task<bool> ExisteNomeNoQuadroAsync(int quadroId, string nome, int? etiquetaIgnoradaId = null);
}
