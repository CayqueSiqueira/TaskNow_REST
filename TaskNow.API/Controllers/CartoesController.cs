using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskNow.DTO.Requests.Cartao;
using TaskNow.DTO.Utils;

namespace TaskNow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CartoesController : ControllerBase
{
    [HttpPost]
    public IActionResult Criar(CartaoCriarRequestDTO request) =>
        StatusCode(StatusCodes.Status501NotImplemented, RetornoDTO<object>.Fail("CRUD de cartoes fica para a Fase 3."));

    [HttpPost("{id:int}/mover")]
    public IActionResult Mover(int id, CartaoMoverRequestDTO request) =>
        StatusCode(StatusCodes.Status501NotImplemented, RetornoDTO<object>.Fail("Mover cartao fica para a Fase 3."));
}