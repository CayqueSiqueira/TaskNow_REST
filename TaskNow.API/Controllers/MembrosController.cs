using TaskNow.BLL.Entities.Interfaces;
using TaskNow.DTO.Requests.Membro;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskNow.DTO.Utils;

namespace TaskNow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MembrosController(IMembroQuadroBLL membroBLL) : ControllerBase
{
    [HttpGet("quadro/{quadroId:int}")]
    public async Task<IActionResult> ListarPorQuadro(int quadroId)
    {
        var retorno = await membroBLL.ListarPorQuadroAsync(quadroId);
        return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [HttpPost("convidar")]
    public async Task<IActionResult> Convidar([FromBody] MembroConvidarRequestDTO request)
    {
        var retorno = await membroBLL.ConvidarMembroAsync(request);
        return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [HttpDelete("{quadroId:int}/usuario/{usuarioId}")]
    public async Task<IActionResult> Remover(int quadroId, string usuarioId)
    {
        var retorno = await membroBLL.RemoverMembroAsync(quadroId, usuarioId);
        return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }
}
