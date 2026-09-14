using Microsoft.AspNetCore.Identity;
using TaskNow.BLL.Entities.Interfaces;
using TaskNow.BLL.Utils.Interfaces;
using TaskNow.DAL.Entities.Interfaces;
using TaskNow.DAO.Entities;
using TaskNow.DAO;
using TaskNow.DTO.Entities;
using TaskNow.DTO.Requests.Membro;
using TaskNow.DTO.Utils;

namespace TaskNow.BLL.Entities;

public class MembroQuadroBLL(
    IMembroQuadroDAL membroDAL,
    IQuadroDAL quadroDAL,
    UserManager<ApplicationUser> userManager,
    IUsuarioContexto usuarioContexto) : IMembroQuadroBLL
{
    public async Task<RetornoDTO<List<MembroQuadroDTO>>> ListarPorQuadroAsync(int quadroId)
    {
        var usuarioId = usuarioContexto.UsuarioId;
        if (string.IsNullOrWhiteSpace(usuarioId)) return RetornoDTO<List<MembroQuadroDTO>>.Fail("Usuário não autenticado.");

        if (!await quadroDAL.UsuarioTemAcessoAsync(quadroId, usuarioId))
            return RetornoDTO<List<MembroQuadroDTO>>.Fail("Você não tem acesso a este quadro.");

        var membros = await membroDAL.ObterPorQuadroAsync(quadroId);
        return RetornoDTO<List<MembroQuadroDTO>>.Ok(membros);
    }

    public async Task<RetornoDTO<MembroQuadroDTO>> ConvidarMembroAsync(MembroConvidarRequestDTO request)
    {
        var usuarioId = usuarioContexto.UsuarioId;
        if (string.IsNullOrWhiteSpace(usuarioId)) return RetornoDTO<MembroQuadroDTO>.Fail("Usuário não autenticado.");

        var quadro = await quadroDAL.GetByIdAsync(request.QuadroId);
        if (quadro == null) return RetornoDTO<MembroQuadroDTO>.Fail("Quadro não encontrado.");

        if (!await membroDAL.EhDonoAsync(request.QuadroId, usuarioId))
            return RetornoDTO<MembroQuadroDTO>.Fail("Apenas o dono do quadro pode convidar novos membros.");

        var convidado = await userManager.FindByEmailAsync(request.Email);
        if (convidado == null) return RetornoDTO<MembroQuadroDTO>.Fail("Usuário com este e-mail não foi encontrado no sistema.");

        if (await membroDAL.EhMembroAsync(request.QuadroId, convidado.Id))
            return RetornoDTO<MembroQuadroDTO>.Fail("Este usuário já é membro do quadro.");

        var novoMembro = new MembroQuadroDTO
        {
            QuadroId = request.QuadroId,
            UsuarioId = convidado.Id,
            Papel = PapelMembro.Membro.ToString()
        };

        var criado = await membroDAL.CreateAsync(novoMembro);
        return RetornoDTO<MembroQuadroDTO>.Ok(criado, "Membro convidado com sucesso.");
    }

    public async Task<RetornoDTO<bool>> RemoverMembroAsync(int quadroId, string usuarioIdParaRemover)
    {
        var usuarioId = usuarioContexto.UsuarioId;
        if (string.IsNullOrWhiteSpace(usuarioId)) return RetornoDTO<bool>.Fail("Usuário não autenticado.");

        var quadro = await quadroDAL.GetByIdAsync(quadroId);
        if (quadro == null) return RetornoDTO<bool>.Fail("Quadro não encontrado.");

        // Pode remover se for o Dono, ou se estiver saindo do proprio quadro
        if (usuarioId != usuarioIdParaRemover && !await membroDAL.EhDonoAsync(quadroId, usuarioId))
            return RetornoDTO<bool>.Fail("Apenas o dono do quadro pode remover outros membros.");

        if (await membroDAL.EhDonoAsync(quadroId, usuarioIdParaRemover))
            return RetornoDTO<bool>.Fail("Não é possível remover o dono do quadro.");

        var removido = await membroDAL.RemoverMembroAsync(quadroId, usuarioIdParaRemover);
        return removido ? RetornoDTO<bool>.Ok(true, "Membro removido com sucesso.") : RetornoDTO<bool>.Fail("Erro ao remover membro.");
    }
}

