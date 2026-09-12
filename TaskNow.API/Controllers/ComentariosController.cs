using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskNow.BLL.Entities.Interfaces;
using TaskNow.DTO.Requests.Comentario;

namespace TaskNow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ComentariosController(IComentarioBLL comentarioBLL) : ControllerBase
{
    [HttpGet("cartao/{cartaoId:int}")]
    public async Task<IActionResult> ListarPorCartao(int cartaoId)
    {
        var retorno = await comentarioBLL.ListarPorCartaoAsync(cartaoId);
        if (!retorno.Sucesso) return BadRequest(retorno);

        return Ok(retorno);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ComentarioCriarRequestDTO request)
    {
        var retorno = await comentarioBLL.CriarAsync(request);
        if (!retorno.Sucesso) return BadRequest(retorno);

        return Ok(retorno);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(int id, [FromBody] ComentarioEditarRequestDTO request)
    {
        var retorno = await comentarioBLL.EditarAsync(id, request);
        if (!retorno.Sucesso) return BadRequest(retorno);

        return Ok(retorno);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var retorno = await comentarioBLL.ExcluirAsync(id);
        if (!retorno.Sucesso) return BadRequest(retorno);

        return Ok(retorno);
    }
}