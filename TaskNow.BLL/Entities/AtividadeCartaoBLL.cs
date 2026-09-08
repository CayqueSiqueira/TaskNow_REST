using TaskNow.BLL.Entities.Interfaces;
using TaskNow.BLL.Utils.Interfaces;
using TaskNow.DAL.Entities.Interfaces;
using TaskNow.DTO.Entities;
using TaskNow.DTO.Utils;

namespace TaskNow.BLL.Entities;

public class AtividadeCartaoBLL(
    IAtividadeCartaoDAL atividadeDAL,
    ICartaoDAL cartaoDAL,
    IListaDAL listaDAL,
    IQuadroDAL quadroDAL,
    IUsuarioContexto usuarioContexto) : IAtividadeCartaoBLL
{
    public async Task<RetornoDTO<List<AtividadeCartaoDTO>>> ListarPorCartaoAsync(int cartaoId)
    {
        // Mesma validacao de seguranca dos comentarios (subindo a arvore)
        var cartao = await cartaoDAL.GetByIdAsync(cartaoId);
        if (cartao == null) return RetornoDTO<List<AtividadeCartaoDTO>>.Fail("Cartão não encontrado.");

        var lista = await listaDAL.GetByIdAsync(cartao.ListaId);
        if (lista == null) return RetornoDTO<List<AtividadeCartaoDTO>>.Fail("Lista não encontrada.");

        var usuarioId = usuarioContexto.UsuarioId;
        if (!await quadroDAL.UsuarioTemAcessoAsync(lista.QuadroId, usuarioId))
            return RetornoDTO<List<AtividadeCartaoDTO>>.Fail("Você não tem acesso a este quadro.");

        var atividades = await atividadeDAL.ObterPorCartaoOrdenadoAsync(cartaoId);
        return RetornoDTO<List<AtividadeCartaoDTO>>.Ok(atividades);
    }

    // Este metodo e interno/auxiliar, entao nao precisa retornar RetornoDTO, apenas faz o trabalho silencioso
    public async Task RegistrarAtividadeAsync(int cartaoId, string tipo, string descricao)
    {
        var atividade = new AtividadeCartaoDTO
        {
            CartaoId = cartaoId,
            Tipo = tipo,
            Descricao = descricao,
            UsuarioId = usuarioContexto.UsuarioId,
            CriadoEm = DateTime.UtcNow
        };

        await atividadeDAL.CreateAsync(atividade);
    }
}
