using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskNow.DTO.Utils;

namespace TaskNow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ComentariosController : ControllerBase
{
    [HttpGet("cartao/{cartaoId:int}")]
    public IActionResult ListarPorCartao(int cartaoId) =>
        StatusCode(StatusCodes.Status501NotImplemented, RetornoDTO<object>.Fail("Comentarios ficam para a Fase 4."));
}