using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TaskNow.DAO;
using TaskNow.DTO.Requests.Auth;
using TaskNow.DTO.Utils;

namespace TaskNow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration) : ControllerBase
{
    [HttpPost("registrar")]
    public async Task<ActionResult<RetornoDTO<AutenticacaoDTO>>> Registrar(RegistrarRequestDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
        {
            return BadRequest(RetornoDTO<AutenticacaoDTO>.Fail("Nome, email e senha sao obrigatorios."));
        }

        var user = new ApplicationUser
        {
            Nome = request.Nome.Trim(),
            UserName = request.Email.Trim(),
            Email = request.Email.Trim()
        };

        var result = await userManager.CreateAsync(user, request.Senha);
        if (!result.Succeeded)
        {
            return BadRequest(RetornoDTO<AutenticacaoDTO>.Fail(string.Join(" ", result.Errors.Select(x => x.Description))));
        }

        return Ok(RetornoDTO<AutenticacaoDTO>.Ok(GerarAutenticacao(user), "Usuario registrado com sucesso."));
    }

    [HttpPost("login")]
    public async Task<ActionResult<RetornoDTO<AutenticacaoDTO>>> Login(LoginRequestDTO request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Senha))
        {
            return Unauthorized(RetornoDTO<AutenticacaoDTO>.Fail("Email ou senha invalidos."));
        }

        return Ok(RetornoDTO<AutenticacaoDTO>.Ok(GerarAutenticacao(user), "Login realizado com sucesso."));
    }

    private AutenticacaoDTO GerarAutenticacao(ApplicationUser user)
    {
        var jwt = configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Nome),
            new Claim(ClaimTypes.Email, user.Email!)
        };

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new AutenticacaoDTO
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UsuarioId = user.Id,
            Nome = user.Nome,
            Email = user.Email!
        };
    }
}