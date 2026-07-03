using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskNow.DTO.Utils;

namespace TaskNow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class EtiquetasController : ControllerBase
{
    [HttpGet("quadro/{quadroId:int}")]
    public IActionResult ListarPorQuadro(int quadroId) =>
        StatusCode(StatusCodes.Status501NotImplemented, RetornoDTO<object>.Fail("Etiquetas ficam para a Fase 4."));
}