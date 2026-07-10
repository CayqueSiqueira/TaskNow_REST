using TaskNow.DAL.Base;
using TaskNow.DAO.Entities;
using TaskNow.DTO.Entities;
using TaskNow.DTO.Utils;

namespace TaskNow.DAL.Entities.Interfaces;

public interface IListaDAL : IBaseDAL<Lista, ListaDTO>
{
    Task<List<ListaDTO>> ObterPorQuadroOrdenadoAsync(int quadroId);
    Task<int> ObterProximaOrdemAsync(int quadroId);
    Task AtualizarOrdensAsync(Dictionary<int, int> ordens);
}
