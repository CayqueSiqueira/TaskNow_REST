using TaskNow.BLL.Entities.Interfaces;
using TaskNow.BLL.Utils.Interfaces;
using TaskNow.DAL.Entities.Interfaces;
using TaskNow.DTO.Entities;
using TaskNow.DTO.Requests.Cartao;
using TaskNow.DTO.Utils;

namespace TaskNow.BLL.Entities;

public class CartaoBLL(
    ICartaoDAL cartaoDAL,
    IListaDAL listaDAL,
    IQuadroDAL quadroDAL,
    IEtiquetaDAL etiquetaDAL,
    IUsuarioContexto usuarioContexto) : ICartaoBLL
{
    public async Task<RetornoDTO<CartaoDTO>> ObterPorIdAsync(int id)
    {
        var usuarioId = usuarioContexto.UsuarioId;
        if (string.IsNullOrWhiteSpace(usuarioId))
        {
            return RetornoDTO<CartaoDTO>.Fail("Usuario nao autenticado.");
        }

        var cartao = await cartaoDAL.GetByIdAsync(id);
        if (cartao == null)
        {
            return RetornoDTO<CartaoDTO>.Fail("Cartao nao encontrado.");
        }

        var lista = await listaDAL.GetByIdAsync(cartao.ListaId);
        if (lista == null)
        {
            return RetornoDTO<CartaoDTO>.Fail("Lista nao encontrada.");
        }

        if (!await quadroDAL.UsuarioTemAcessoAsync(lista.QuadroId, usuarioId))
        {
            return RetornoDTO<CartaoDTO>.Fail("Voce nao tem acesso a este quadro.");
        }

        return RetornoDTO<CartaoDTO>.Ok(cartao);
    }

    public async Task<RetornoDTO<CartaoDTO>> CriarAsync(CartaoCriarRequestDTO request)
    {
        var usuarioId = usuarioContexto.UsuarioId;
        if (string.IsNullOrWhiteSpace(usuarioId))
        {
            return RetornoDTO<CartaoDTO>.Fail("Usuario nao autenticado.");
        }

        if (string.IsNullOrWhiteSpace(request.Titulo))
        {
            return RetornoDTO<CartaoDTO>.Fail("Titulo do cartao e obrigatorio.");
        }

        if (request.Titulo.Length > 160)
        {
            return RetornoDTO<CartaoDTO>.Fail("Titulo do cartao deve ter no maximo 160 caracteres.");
        }

        if (request.Descricao != null && request.Descricao.Length > 2000)
        {
            return RetornoDTO<CartaoDTO>.Fail("Descricao do cartao deve ter no maximo 2000 caracteres.");
        }

        var lista = await listaDAL.GetByIdAsync(request.ListaId);
        if (lista == null)
        {
            return RetornoDTO<CartaoDTO>.Fail("Lista nao encontrada.");
        }

        if (!await quadroDAL.UsuarioTemAcessoAsync(lista.QuadroId, usuarioId))
        {
            return RetornoDTO<CartaoDTO>.Fail("Voce nao tem acesso a este quadro.");
        }

        if (!string.IsNullOrWhiteSpace(request.ResponsavelId))
        {
            if (!await quadroDAL.UsuarioTemAcessoAsync(lista.QuadroId, request.ResponsavelId))
            {
                return RetornoDTO<CartaoDTO>.Fail("O responsavel informado nao tem acesso a este quadro.");
            }
        }

        var cartao = await cartaoDAL.CreateAsync(new CartaoDTO
        {
            ListaId = request.ListaId,
            Titulo = request.Titulo.Trim(),
            Descricao = string.IsNullOrWhiteSpace(request.Descricao) ? null : request.Descricao.Trim(),
            Ordem = await cartaoDAL.ObterProximaOrdemAsync(request.ListaId),
            Prazo = request.Prazo.HasValue ? DateTime.SpecifyKind(request.Prazo.Value, DateTimeKind.Utc) : null,
            ResponsavelId = string.IsNullOrWhiteSpace(request.ResponsavelId) ? null : request.ResponsavelId
        });

        return RetornoDTO<CartaoDTO>.Ok(cartao, "Cartao criado com sucesso.");
    }

    public async Task<RetornoDTO<CartaoDTO>> EditarAsync(int id, CartaoEditarRequestDTO request)
    {
        var usuarioId = usuarioContexto.UsuarioId;
        if (string.IsNullOrWhiteSpace(usuarioId))
        {
            return RetornoDTO<CartaoDTO>.Fail("Usuario nao autenticado.");
        }

        if (string.IsNullOrWhiteSpace(request.Titulo))
        {
            return RetornoDTO<CartaoDTO>.Fail("Titulo do cartao e obrigatorio.");
        }

        if (request.Titulo.Length > 160)
        {
            return RetornoDTO<CartaoDTO>.Fail("Titulo do cartao deve ter no maximo 160 caracteres.");
        }

        if (request.Descricao != null && request.Descricao.Length > 2000)
        {
            return RetornoDTO<CartaoDTO>.Fail("Descricao do cartao deve ter no maximo 2000 caracteres.");
        }

        var cartao = await cartaoDAL.GetByIdAsync(id);
        if (cartao == null)
        {
            return RetornoDTO<CartaoDTO>.Fail("Cartao nao encontrado.");
        }

        var lista = await listaDAL.GetByIdAsync(cartao.ListaId);
        if (lista == null)
        {
            return RetornoDTO<CartaoDTO>.Fail("Lista nao encontrada.");
        }

        if (!await quadroDAL.UsuarioTemAcessoAsync(lista.QuadroId, usuarioId))
        {
            return RetornoDTO<CartaoDTO>.Fail("Voce nao tem acesso a este quadro.");
        }

        if (!string.IsNullOrWhiteSpace(request.ResponsavelId))
        {
            if (!await quadroDAL.UsuarioTemAcessoAsync(lista.QuadroId, request.ResponsavelId))
            {
                return RetornoDTO<CartaoDTO>.Fail("O responsavel informado nao tem acesso a este quadro.");
            }
        }

        cartao.Titulo = request.Titulo.Trim();
        cartao.Descricao = string.IsNullOrWhiteSpace(request.Descricao) ? null : request.Descricao.Trim();
        cartao.Prazo = request.Prazo.HasValue ? DateTime.SpecifyKind(request.Prazo.Value, DateTimeKind.Utc) : null;
        cartao.ResponsavelId = string.IsNullOrWhiteSpace(request.ResponsavelId) ? null : request.ResponsavelId;

        var cartaoEditado = await cartaoDAL.EditAsync(id, cartao);
        if (cartaoEditado == null)
        {
            return RetornoDTO<CartaoDTO>.Fail("Cartao nao encontrado.");
        }

        return RetornoDTO<CartaoDTO>.Ok(cartaoEditado, "Cartao atualizado com sucesso.");
    }

    public async Task<RetornoDTO<bool>> ExcluirAsync(int id)
    {
        var usuarioId = usuarioContexto.UsuarioId;
        if (string.IsNullOrWhiteSpace(usuarioId))
        {
            return RetornoDTO<bool>.Fail("Usuario nao autenticado.");
        }

        var cartao = await cartaoDAL.GetByIdAsync(id);
        if (cartao == null)
        {
            return RetornoDTO<bool>.Fail("Cartao nao encontrado.");
        }

        var lista = await listaDAL.GetByIdAsync(cartao.ListaId);
        if (lista == null)
        {
            return RetornoDTO<bool>.Fail("Lista nao encontrada.");
        }

        if (!await quadroDAL.UsuarioTemAcessoAsync(lista.QuadroId, usuarioId))
        {
            return RetornoDTO<bool>.Fail("Voce nao tem acesso a este quadro.");
        }

        var deletado = await cartaoDAL.DeleteAsync(id);
        return deletado
            ? RetornoDTO<bool>.Ok(true, "Cartao excluido com sucesso.")
            : RetornoDTO<bool>.Fail("Erro ao excluir cartao.");
    }

    public Task<RetornoDTO<bool>> MoverAsync(int cartaoId, CartaoMoverRequestDTO request)
    {
        return Task.FromResult(RetornoDTO<bool>.Fail("Mover cartao fica para a Fase 3."));
    }

    public async Task<RetornoDTO<bool>> AssociarEtiquetaAsync(int cartaoId, int etiquetaId)
    {
        var usuarioId = usuarioContexto.UsuarioId;
        if (string.IsNullOrWhiteSpace(usuarioId))
        {
            return RetornoDTO<bool>.Fail("Usuario nao autenticado.");
        }

        var cartao = await cartaoDAL.GetByIdAsync(cartaoId);
        if (cartao == null)
            return RetornoDTO<bool>.Fail("Cartao nao encontrado.");
        var lista = await listaDAL.GetByIdAsync(cartao.ListaId);
        if (lista == null)
            return RetornoDTO<bool>.Fail("Lista nao encontrada.");

        var etiqueta = await etiquetaDAL.GetByIdAsync(etiquetaId);
        if (etiqueta == null)
            return RetornoDTO<bool>.Fail("Etiqueta nao encontrada.");

        if (lista.QuadroId != etiqueta.QuadroId)
        {
            return RetornoDTO<bool>.Fail("A etiqueta e o cartao nao pertencem ao mesmo quadro.");
        }

        if (!await quadroDAL.UsuarioTemAcessoAsync(lista.QuadroId, usuarioId))
        {
            return RetornoDTO<bool>.Fail("Voce nao tem acesso a este quadro.");
        }

        if (await cartaoDAL.CartaoPossuiEtiquetaAsync(cartaoId, etiquetaId))
        {
            return RetornoDTO<bool>.Ok(true, "Etiqueta ja associada."); 
        }

        var sucesso = await cartaoDAL.AssociarEtiquetaAsync(cartaoId, etiquetaId);
        return sucesso
            ? RetornoDTO<bool>.Ok(true, "Etiqueta associada com sucesso.")
            : RetornoDTO<bool>.Fail("Erro ao associar etiqueta.");
    }

    public async Task<RetornoDTO<bool>> RemoverEtiquetaAsync(int cartaoId, int etiquetaId)
    {
        var usuarioId = usuarioContexto.UsuarioId;
        if (string.IsNullOrWhiteSpace(usuarioId))
        {
            return RetornoDTO<bool>.Fail("Usuario nao autenticado.");
        }

        var cartao = await cartaoDAL.GetByIdAsync(cartaoId);
        if (cartao == null)
            return RetornoDTO<bool>.Fail("Cartao nao encontrado.");

        var lista = await listaDAL.GetByIdAsync(cartao.ListaId);
        if (lista == null)
            return RetornoDTO<bool>.Fail("Lista nao encontrada.");

        if (!await quadroDAL.UsuarioTemAcessoAsync(lista.QuadroId, usuarioId))
        {
            return RetornoDTO<bool>.Fail("Voce nao tem acesso a este quadro.");
        }
        var sucesso = await cartaoDAL.RemoverEtiquetaAsync(cartaoId, etiquetaId);

        return sucesso
            ? RetornoDTO<bool>.Ok(true, "Etiqueta removida com sucesso.")
            : RetornoDTO<bool>.Ok(true, "A etiqueta nao estava associada ao cartao.");
    }
}
