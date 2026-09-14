using TaskNow.DTO.Entities;
using TaskNow.DTO.Requests.Membro;
using TaskNow.DTO.Utils;

namespace TaskNow.BLL.Entities.Interfaces;

public interface IMembroQuadroBLL
{
    Task<RetornoDTO<List<MembroQuadroDTO>>> ListarPorQuadroAsync(int quadroId);
    Task<RetornoDTO<MembroQuadroDTO>> ConvidarMembroAsync(MembroConvidarRequestDTO request);
    Task<RetornoDTO<bool>> RemoverMembroAsync(int quadroId, string usuarioIdParaRemover);
}