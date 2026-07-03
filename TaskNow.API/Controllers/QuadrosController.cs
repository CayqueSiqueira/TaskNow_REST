using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskNow.BLL.Entities.Interfaces;
using TaskNow.DTO.Requests.Quadro;

namespace TaskNow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class QuadrosController(IQuadroBLL quadroBLL) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await quadroBLL.ListarAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obter(int id)
    {
        var retorno = await quadroBLL.ObterAsync(id);
        return retorno.Sucesso ? Ok(retorno) : NotFound(retorno);
    }

    [HttpPost]
    public async Task<IActionResult> Criar(QuadroCriarRequestDTO request)
    {
        var retorno = await quadroBLL.CriarAsync(request);
        return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(int id, QuadroEditarRequestDTO request)
    {
        var retorno = await quadroBLL.EditarAsync(id, request);
        return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var retorno = await quadroBLL.ExcluirAsync(id);
        return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }
}