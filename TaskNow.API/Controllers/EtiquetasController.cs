using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskNow.BLL.Entities.Interfaces;
using TaskNow.DTO.Requests.Etiqueta;

namespace TaskNow.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class EtiquetasController(IEtiquetaBLL etiquetaBLL) : ControllerBase
{
    [HttpGet("quadro/{quadroId:int}")]
    public async Task<IActionResult> ObterPorQuadro(int quadroId)
    {
        var retorno = await etiquetaBLL.ListarPorQuadroAsync(quadroId);
        if (!retorno.Sucesso)
            return BadRequest(retorno);

        return Ok(retorno);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] EtiquetaCriarRequestDTO request)
    {
        var retorno = await etiquetaBLL.CriarAsync(request);
        if (!retorno.Sucesso)
            return BadRequest(retorno);

        return Ok(retorno);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(int id, [FromBody] EtiquetaCriarRequestDTO request)
    {
        var retorno = await etiquetaBLL.EditarAsync(id, request);
        if (!retorno.Sucesso)
            return BadRequest(retorno);

        return Ok(retorno);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var retorno = await etiquetaBLL.ExcluirAsync(id);
        if (!retorno.Sucesso)
            return BadRequest(retorno);

        return Ok(retorno);
    }
}