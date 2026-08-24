using System.Text.RegularExpressions;
using TaskNow.BLL.Entities.Interfaces;
using TaskNow.BLL.Utils.Interfaces;
using TaskNow.DAL.Entities.Interfaces;
using TaskNow.DTO.Entities;
using TaskNow.DTO.Requests.Etiqueta;
using TaskNow.DTO.Utils;

namespace TaskNow.BLL.Entities;

public class EtiquetaBLL(
    IEtiquetaDAL etiquetaDAL,
    IQuadroDAL quadroDAL,
    IUsuarioContexto usuarioContexto) : IEtiquetaBLL
{
    private static readonly Regex CorRegex = new Regex("^#[0-9a-fA-F]{6}$", RegexOptions.Compiled);

    public async Task<RetornoDTO<List<EtiquetaDTO>>> ListarPorQuadroAsync(int quadroId)
    {
        var usuarioId = usuarioContexto.UsuarioId;
        if (string.IsNullOrWhiteSpace(usuarioId))
        {
            return RetornoDTO<List<EtiquetaDTO>>.Fail("Usuario nao autenticado.");
        }

        if (!await quadroDAL.UsuarioTemAcessoAsync(quadroId, usuarioId))
        {
            return RetornoDTO<List<EtiquetaDTO>>.Fail("Voce nao tem acesso a este quadro.");
        }

        var etiquetas = await etiquetaDAL.ObterPorQuadroAsync(quadroId);
        return RetornoDTO<List<EtiquetaDTO>>.Ok(etiquetas);
    }

    public async Task<RetornoDTO<EtiquetaDTO>> CriarAsync(EtiquetaCriarRequestDTO request)
    {
        var validacao = await ValidarDadosAcessoAsync(request.QuadroId, request.Nome, request.Cor);
        if (!validacao.Sucesso) return RetornoDTO<EtiquetaDTO>.Fail(validacao.Mensagem);

        if (await etiquetaDAL.ExisteNomeNoQuadroAsync(request.QuadroId, request.Nome.Trim()))
        {
            return RetornoDTO<EtiquetaDTO>.Fail("Ja existe uma etiqueta com este nome neste quadro.");
        }

        var etiqueta = await etiquetaDAL.CreateAsync(new EtiquetaDTO
        {
            QuadroId = request.QuadroId,
            Nome = request.Nome.Trim(),
            Cor = request.Cor.ToUpper()
        });

        return RetornoDTO<EtiquetaDTO>.Ok(etiqueta, "Etiqueta criada com sucesso.");
    }

    public async Task<RetornoDTO<EtiquetaDTO>> EditarAsync(int id, EtiquetaCriarRequestDTO request)
    {
        var etiquetaExistente = await etiquetaDAL.GetByIdAsync(id);
        if (etiquetaExistente == null)
        {
            return RetornoDTO<EtiquetaDTO>.Fail("Etiqueta nao encontrada.");
        }

        // Usa o quadroId da etiqueta real para impedir que ela seja movida para outro quadro
        var quadroId = etiquetaExistente.QuadroId; 

        var validacao = await ValidarDadosAcessoAsync(quadroId, request.Nome, request.Cor);
        if (!validacao.Sucesso) return RetornoDTO<EtiquetaDTO>.Fail(validacao.Mensagem);

        if (await etiquetaDAL.ExisteNomeNoQuadroAsync(quadroId, request.Nome.Trim(), id))
        {
            return RetornoDTO<EtiquetaDTO>.Fail("Ja existe uma etiqueta com este nome neste quadro.");
        }

        etiquetaExistente.Nome = request.Nome.Trim();
        etiquetaExistente.Cor = request.Cor.ToUpper();

        var etiquetaEditada = await etiquetaDAL.EditAsync(id, etiquetaExistente);
        return RetornoDTO<EtiquetaDTO>.Ok(etiquetaEditada, "Etiqueta atualizada com sucesso.");
    }

    public async Task<RetornoDTO<bool>> ExcluirAsync(int id)
    {
        var usuarioId = usuarioContexto.UsuarioId;
        if (string.IsNullOrWhiteSpace(usuarioId))
        {
            return RetornoDTO<bool>.Fail("Usuario nao autenticado.");
        }

        var etiqueta = await etiquetaDAL.GetByIdAsync(id);
        if (etiqueta == null)
        {
            return RetornoDTO<bool>.Fail("Etiqueta nao encontrada.");
        }

        if (!await quadroDAL.UsuarioTemAcessoAsync(etiqueta.QuadroId, usuarioId))
        {
            return RetornoDTO<bool>.Fail("Voce nao tem acesso a este quadro.");
        }

        var deletado = await etiquetaDAL.DeleteAsync(id);
        return deletado
            ? RetornoDTO<bool>.Ok(true, "Etiqueta excluida com sucesso.")
            : RetornoDTO<bool>.Fail("Erro ao excluir etiqueta.");
    }

    private async Task<RetornoDTO<bool>> ValidarDadosAcessoAsync(int quadroId, string nome, string cor)
    {
        var usuarioId = usuarioContexto.UsuarioId;
        if (string.IsNullOrWhiteSpace(usuarioId))
        {
            return RetornoDTO<bool>.Fail("Usuario nao autenticado.");
        }

        if (!await quadroDAL.UsuarioTemAcessoAsync(quadroId, usuarioId))
        {
            return RetornoDTO<bool>.Fail("Voce nao tem acesso a este quadro.");
        }

        if (string.IsNullOrWhiteSpace(nome))
        {
            return RetornoDTO<bool>.Fail("Nome da etiqueta e obrigatorio.");
        }

        if (nome.Trim().Length > 80)
        {
            return RetornoDTO<bool>.Fail("Nome da etiqueta deve ter no maximo 80 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(cor))
        {
            return RetornoDTO<bool>.Fail("Cor da etiqueta e obrigatoria.");
        }

        if (!CorRegex.IsMatch(cor))
        {
            return RetornoDTO<bool>.Fail("Cor da etiqueta deve estar no formato #RRGGBB.");
        }

        return RetornoDTO<bool>.Ok(true);
    }
}
