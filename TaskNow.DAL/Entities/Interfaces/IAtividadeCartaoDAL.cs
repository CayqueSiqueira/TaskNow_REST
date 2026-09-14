using TaskNow.DAL.Base;
using TaskNow.DAO.Entities;
using TaskNow.DTO.Entities;

namespace TaskNow.DAL.Entities.Interfaces;

public interface IAtividadeCartaoDAL : IBaseDAL<AtividadeCartao, AtividadeCartaoDTO>
{
    Task<List<AtividadeCartaoDTO>> ObterPorCartaoOrdenadoAsync(int cartaoId);
}
