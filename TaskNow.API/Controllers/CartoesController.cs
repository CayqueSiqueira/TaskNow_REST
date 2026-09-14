using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskNow.BLL.Entities.Interfaces;
using TaskNow.DTO.Requests.Cartao;
using TaskNow.DTO.Utils;

namespace TaskNow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CartoesController(ICartaoBLL cartaoBLL) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var retorno = await cartaoBLL.ObterPorIdAsync(id);
        return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [HttpPost]
    public async Task<IActionResult> Criar(CartaoCriarRequestDTO request)
    {
        var retorno = await cartaoBLL.CriarAsync(request);
        return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(int id, CartaoEditarRequestDTO request)
    {
        var retorno = await cartaoBLL.EditarAsync(id, request);
        return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var retorno = await cartaoBLL.ExcluirAsync(id);
        return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [HttpGet("{id:int}/atividades")]
    public async Task<IActionResult> ObterAtividades(int id, [FromServices] IAtividadeCartaoBLL atividadeBLL)
    {
        var retorno = await atividadeBLL.ListarPorCartaoAsync(id);
        return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [HttpPost("{id:int}/mover")]
    public async Task<IActionResult> Mover(int id, CartaoMoverRequestDTO request)
    {
        var retorno = await cartaoBLL.MoverAsync(id, request);
        return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [HttpPost("{id:int}/etiquetas/{etiquetaId:int}")]
    public async Task<IActionResult> AssociarEtiqueta(int id, int etiquetaId)
    {
        var retorno = await cartaoBLL.AssociarEtiquetaAsync(id, etiquetaId);
        return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }

    [HttpDelete("{id:int}/etiquetas/{etiquetaId:int}")]
    public async Task<IActionResult> RemoverEtiqueta(int id, int etiquetaId)
    {
        var retorno = await cartaoBLL.RemoverEtiquetaAsync(id, etiquetaId);
        return retorno.Sucesso ? Ok(retorno) : BadRequest(retorno);
    }
}