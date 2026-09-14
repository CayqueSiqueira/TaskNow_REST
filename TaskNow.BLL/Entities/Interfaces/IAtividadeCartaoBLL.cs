using TaskNow.DTO.Entities;
using TaskNow.DTO.Utils;

namespace TaskNow.BLL.Entities.Interfaces;

public interface IAtividadeCartaoBLL
{
    Task<RetornoDTO<List<AtividadeCartaoDTO>>> ListarPorCartaoAsync(int cartaoId);
    Task RegistrarAtividadeAsync(int cartaoId, string tipo, string descricao);
}
