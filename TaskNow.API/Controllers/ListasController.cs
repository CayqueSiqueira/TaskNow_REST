using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskNow.BLL.Entities.Interfaces;
using TaskNow.DTO.Requests.Lista;

namespace TaskNow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ListasController(IListaBLL listaBLL) : ControllerBase
{
    [HttpGet("quadro/{quadroId:int}")]
    public async Task<IActionResult> ListarPorQuadro(int quadroId)
    {
        var retorno = await listaBLL.ListarPorQuadroAsync(quadroId);
        return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [HttpPost]
    public async Task<IActionResult> Criar(ListaCriarRequestDTO request)
    {
        var retorno = await listaBLL.CriarAsync(request);
        return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [HttpPost("quadro/{quadroId:int}/padrao")]
    public async Task<IActionResult> CriarPadrao(int quadroId)
    {
        var retorno = await listaBLL.CriarPadraoAsync(quadroId);
        return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [HttpPost("quadro/{quadroId:int}/reordenar")]
    public async Task<IActionResult> Reordenar(int quadroId, ListaReordenarRequestDTO request)
    {
        var retorno = await listaBLL.ReordenarAsync(quadroId, request);
        return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(int id, ListaCriarRequestDTO request)
    {
        var retorno = await listaBLL.EditarAsync(id, request);
        return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var retorno = await listaBLL.ExcluirAsync(id);
        return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }
}