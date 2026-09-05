using TaskNow.BLL.Entities.Interfaces;
using TaskNow.BLL.Utils.Interfaces;
using TaskNow.DAL.Entities.Interfaces;
using TaskNow.DTO.Entities;
using TaskNow.DTO.Requests.Comentario;
using TaskNow.DTO.Utils;

namespace TaskNow.BLL.Entities;

public class ComentarioBLL(
    IComentarioDAL comentarioDAL,
    ICartaoDAL cartaoDAL,
    IListaDAL listaDAL,
    IQuadroDAL quadroDAL,
    IUsuarioContexto usuarioContexto) : IComentarioBLL
{
    public async Task<RetornoDTO<List<ComentarioDTO>>> ListarPorCartaoAsync(int cartaoId)
    {
        var quadroId = await ObterQuadroIdPeloCartaoAsync(cartaoId);
        if (quadroId == null) return RetornoDTO<List<ComentarioDTO>>.Fail("Comentario nao encontrado.");

        if (!await quadroDAL.UsuarioTemAcessoAsync(quadroId.Value, usuarioContexto.UsuarioId))
            return RetornoDTO<List<ComentarioDTO>>.Fail("Voce nao tem acesso a este quadro.");

        var comentarios = await comentarioDAL.ObterPorCartaoAsync(cartaoId);
        return RetornoDTO<List<ComentarioDTO>>.Ok(comentarios);
    }

    public async Task<RetornoDTO<ComentarioDTO>> CriarAsync(ComentarioCriarRequestDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Texto))
            return RetornoDTO<ComentarioDTO>.Fail("Texto do comentario e obrigatorio.");

        if (request.Texto.Length > 2000)
            return RetornoDTO<ComentarioDTO>.Fail("Texto do comentario deve ter no maximo 2000 caracteres.");

        var quadroId = await ObterQuadroIdPeloCartaoAsync(request.CartaoId);
        if (quadroId == null) return RetornoDTO<ComentarioDTO>.Fail("Cartao nao encontrado.");

        var usuarioId = usuarioContexto.UsuarioId;
        if (!await quadroDAL.UsuarioTemAcessoAsync(quadroId.Value, usuarioId))
            return RetornoDTO<ComentarioDTO>.Fail("Voce nao tem acesso a este quadro.");

        var dtoParaCriar = new ComentarioDTO
        {
            CartaoId = request.CartaoId,
            Texto = request.Texto,
            AutorId = usuarioId, 
            CriadoEm = DateTime.UtcNow
        };

        var novoComentario = await comentarioDAL.CreateAsync(dtoParaCriar);
        return RetornoDTO<ComentarioDTO>.Ok(novoComentario);
    }

    public async Task<RetornoDTO<ComentarioDTO>> EditarAsync(int id, ComentarioEditarRequestDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Texto))
            return RetornoDTO<ComentarioDTO>.Fail("Texto do comentario e obrigatorio.");

        if (request.Texto.Length > 2000)
            return RetornoDTO<ComentarioDTO>.Fail("Texto do comentario deve ter no maximo 2000 caracteres.");

        var comentario = await comentarioDAL.GetByIdAsync(id);
        if (comentario == null) return RetornoDTO<ComentarioDTO>.Fail("Comentario nao encontrado.");

        var quadroId = await ObterQuadroIdPeloCartaoAsync(comentario.CartaoId);
        var quadro = await quadroDAL.GetByIdAsync(quadroId!.Value);

        var usuarioId = usuarioContexto.UsuarioId;
        if (comentario.AutorId != usuarioId && quadro!.DonoId != usuarioId)
        {
            return RetornoDTO<ComentarioDTO>.Fail("Somente o autor ou dono do quadro pode alterar este comentario.");
        }

        comentario.Texto = request.Texto;
        var editado = await comentarioDAL.EditAsync(id, comentario);
        return RetornoDTO<ComentarioDTO>.Ok(editado);
    }

    public async Task<RetornoDTO<bool>> ExcluirAsync(int id)
    {
        var comentario = await comentarioDAL.GetByIdAsync(id);
        if (comentario == null) return RetornoDTO<bool>.Fail("Comentario nao encontrado.");

        var quadroId = await ObterQuadroIdPeloCartaoAsync(comentario.CartaoId);
        var quadro = await quadroDAL.GetByIdAsync(quadroId!.Value);

        var usuarioId = usuarioContexto.UsuarioId;

        if (comentario.AutorId != usuarioId && quadro!.DonoId != usuarioId)
        {
            return RetornoDTO<bool>.Fail("Somente o autor ou dono do quadro pode alterar este comentario.");
        }

        await comentarioDAL.DeleteAsync(id);
        return RetornoDTO<bool>.Ok(true);
    }

    private async Task<int?> ObterQuadroIdPeloCartaoAsync(int cartaoId)
    {
        var cartao = await cartaoDAL.GetByIdAsync(cartaoId);
        if (cartao == null) return null;

        var lista = await listaDAL.GetByIdAsync(cartao.ListaId);
        return lista?.QuadroId;
    }
}