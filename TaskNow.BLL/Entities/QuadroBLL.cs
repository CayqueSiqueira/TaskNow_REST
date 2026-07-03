using TaskNow.BLL.Entities.Interfaces;
using TaskNow.BLL.Utils.Interfaces;
using TaskNow.DAL.Entities.Interfaces;
using TaskNow.DTO.Entities;
using TaskNow.DTO.Requests.Quadro;
using TaskNow.DTO.Utils;

namespace TaskNow.BLL.Entities;

public class QuadroBLL(IQuadroDAL quadroDAL, IUsuarioContexto usuarioContexto) : IQuadroBLL
{
    public async Task<RetornoDTO<List<QuadroDTO>>> ListarAsync()
    {
        var usuarioId = usuarioContexto.UsuarioId;
        if (string.IsNullOrWhiteSpace(usuarioId))
        {
            return RetornoDTO<List<QuadroDTO>>.Fail("Usuario nao autenticado.");
        }

        var quadros = await quadroDAL.ObterPorUsuarioAsync(usuarioId);
        return RetornoDTO<List<QuadroDTO>>.Ok(quadros);
    }

    public async Task<RetornoDTO<QuadroDTO>> ObterAsync(int id)
    {
        var usuarioId = usuarioContexto.UsuarioId;
        if (string.IsNullOrWhiteSpace(usuarioId))
        {
            return RetornoDTO<QuadroDTO>.Fail("Usuario nao autenticado.");
        }

        if (!await quadroDAL.UsuarioTemAcessoAsync(id, usuarioId))
        {
            return RetornoDTO<QuadroDTO>.Fail("Voce nao tem acesso a este quadro.");
        }

        var quadro = await quadroDAL.GetByIdAsync(id);
        return quadro is null
            ? RetornoDTO<QuadroDTO>.Fail("Quadro nao encontrado.")
            : RetornoDTO<QuadroDTO>.Ok(quadro);
    }

    public async Task<RetornoDTO<QuadroDTO>> CriarAsync(QuadroCriarRequestDTO request)
    {
        var usuarioId = usuarioContexto.UsuarioId;
        if (string.IsNullOrWhiteSpace(usuarioId))
        {
            return RetornoDTO<QuadroDTO>.Fail("Usuario nao autenticado.");
        }

        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            return RetornoDTO<QuadroDTO>.Fail("Nome do quadro e obrigatorio.");
        }

        var quadro = await quadroDAL.CreateAsync(new QuadroDTO
        {
            Nome = request.Nome.Trim(),
            Descricao = string.IsNullOrWhiteSpace(request.Descricao) ? null : request.Descricao.Trim(),
            DonoId = usuarioId
        });

        return RetornoDTO<QuadroDTO>.Ok(quadro, "Quadro criado com sucesso.");
    }

    public async Task<RetornoDTO<QuadroDTO>> EditarAsync(int id, QuadroEditarRequestDTO request)
    {
        var usuarioId = usuarioContexto.UsuarioId;
        if (string.IsNullOrWhiteSpace(usuarioId))
        {
            return RetornoDTO<QuadroDTO>.Fail("Usuario nao autenticado.");
        }

        if (!await quadroDAL.UsuarioTemAcessoAsync(id, usuarioId))
        {
            return RetornoDTO<QuadroDTO>.Fail("Voce nao tem acesso a este quadro.");
        }

        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            return RetornoDTO<QuadroDTO>.Fail("Nome do quadro e obrigatorio.");
        }

        var quadro = await quadroDAL.EditAsync(id, new QuadroDTO
        {
            Nome = request.Nome.Trim(),
            Descricao = string.IsNullOrWhiteSpace(request.Descricao) ? null : request.Descricao.Trim()
        });

        return quadro is null
            ? RetornoDTO<QuadroDTO>.Fail("Quadro nao encontrado.")
            : RetornoDTO<QuadroDTO>.Ok(quadro, "Quadro atualizado com sucesso.");
    }

    public async Task<RetornoDTO<bool>> ExcluirAsync(int id)
    {
        var usuarioId = usuarioContexto.UsuarioId;
        if (string.IsNullOrWhiteSpace(usuarioId))
        {
            return RetornoDTO<bool>.Fail("Usuario nao autenticado.");
        }

        var quadro = await quadroDAL.GetByIdAsync(id);
        if (quadro is null)
        {
            return RetornoDTO<bool>.Fail("Quadro nao encontrado.");
        }

        if (quadro.DonoId != usuarioId)
        {
            return RetornoDTO<bool>.Fail("Somente o dono pode excluir o quadro.");
        }

        return RetornoDTO<bool>.Ok(await quadroDAL.DeleteAsync(id), "Quadro excluido com sucesso.");
    }
}