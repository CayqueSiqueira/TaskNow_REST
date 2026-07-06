using TaskNow.BLL.Entities.Interfaces;
using TaskNow.BLL.Utils.Interfaces;
using TaskNow.DAL.Entities.Interfaces;
using TaskNow.DTO.Entities;
using TaskNow.DTO.Requests.Lista;
using TaskNow.DTO.Utils;

namespace TaskNow.BLL.Entities;

public class ListaBLL(IListaDAL listaDAL, IQuadroDAL quadroDAL, IUsuarioContexto usuarioContexto) : IListaBLL
{
    public async Task<RetornoDTO<List<ListaDTO>>> ListarPorQuadroAsync(int quadroId)
    {
        var permissao = await ValidarAcessoQuadroAsync(quadroId);
        if (!permissao.Sucesso)
        {
            return RetornoDTO<List<ListaDTO>>.Fail(permissao.Mensagem!);
        }

        var listas = await listaDAL.ObterPorQuadroOrdenadoAsync(quadroId);
        return RetornoDTO<List<ListaDTO>>.Ok(listas);
    }

    public async Task<RetornoDTO<ListaDTO>> CriarAsync(ListaCriarRequestDTO request)
    {
        var permissao = await ValidarAcessoQuadroAsync(request.QuadroId);
        if (!permissao.Sucesso)
        {
            return RetornoDTO<ListaDTO>.Fail(permissao.Mensagem!);
        }

        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            return RetornoDTO<ListaDTO>.Fail("Nome da lista e obrigatorio.");
        }

        var lista = await listaDAL.CreateAsync(new ListaDTO
        {
            QuadroId = request.QuadroId,
            Nome = request.Nome.Trim(),
            Ordem = await listaDAL.ObterProximaOrdemAsync(request.QuadroId)
        });

        return RetornoDTO<ListaDTO>.Ok(lista, "Lista criada com sucesso.");
    }

    public async Task<RetornoDTO<ListaDTO>> EditarAsync(int id, ListaCriarRequestDTO request)
    {
        var listaAtual = await listaDAL.GetByIdAsync(id);
        if (listaAtual is null)
        {
            return RetornoDTO<ListaDTO>.Fail("Lista nao encontrada.");
        }

        var permissao = await ValidarAcessoQuadroAsync(listaAtual.QuadroId);
        if (!permissao.Sucesso)
        {
            return RetornoDTO<ListaDTO>.Fail(permissao.Mensagem!);
        }

        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            return RetornoDTO<ListaDTO>.Fail("Nome da lista e obrigatorio.");
        }

        var lista = await listaDAL.EditAsync(id, new ListaDTO { Nome = request.Nome.Trim() });
        return lista is null
            ? RetornoDTO<ListaDTO>.Fail("Lista nao encontrada.")
            : RetornoDTO<ListaDTO>.Ok(lista, "Lista atualizada com sucesso.");
    }

    public async Task<RetornoDTO<bool>> ExcluirAsync(int id)
    {
        var lista = await listaDAL.GetByIdAsync(id);
        if (lista is null)
        {
            return RetornoDTO<bool>.Fail("Lista nao encontrada.");
        }

        var permissao = await ValidarAcessoQuadroAsync(lista.QuadroId);
        if (!permissao.Sucesso)
        {
            return RetornoDTO<bool>.Fail(permissao.Mensagem!);
        }

        return RetornoDTO<bool>.Ok(await listaDAL.DeleteAsync(id), "Lista excluida com sucesso.");
    }

    public async Task<RetornoDTO<List<ListaDTO>>> CriarPadraoAsync(int quadroId)
    {
        var permissao = await ValidarAcessoQuadroAsync(quadroId);
        if (!permissao.Sucesso)
        {
            return RetornoDTO<List<ListaDTO>>.Fail(permissao.Mensagem!);
        }

        var listasExistentes = await listaDAL.ObterPorQuadroOrdenadoAsync(quadroId);
        var nomesPadrao = new List<string> { "A fazer", "Em andamento", "Em teste", "Concluido" };

        var nomesParaCriar = nomesPadrao
            .Where(nome => !listasExistentes.Any(le => le.Nome.Trim().Equals(nome, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        if (nomesParaCriar.Count > 0)
        {
            int proximaOrdem = await listaDAL.ObterProximaOrdemAsync(quadroId);
            foreach (var nome in nomesParaCriar)
            {
                await listaDAL.CreateAsync(new ListaDTO
                {
                    QuadroId = quadroId,
                    Nome = nome,
                    Ordem = proximaOrdem++
                });
            }
        }

        var listasAtualizadas = await listaDAL.ObterPorQuadroOrdenadoAsync(quadroId);
        return RetornoDTO<List<ListaDTO>>.Ok(listasAtualizadas, "Listas padrao processadas com sucesso.");
    }

    private async Task<RetornoDTO<bool>> ValidarAcessoQuadroAsync(int quadroId)
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

        return RetornoDTO<bool>.Ok(true);
    }
}