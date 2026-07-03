using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TaskNow.BLL.Utils.Interfaces;

namespace TaskNow.BLL.Utils;

public class UsuarioContexto(IHttpContextAccessor httpContextAccessor) : IUsuarioContexto
{
    public string? UsuarioId =>
        httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
}