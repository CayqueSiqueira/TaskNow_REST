using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TaskNow.API.Hubs;

[Authorize]
public class QuadroHub : Hub
{
    public async Task EntrarNoQuadro(int quadroId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GrupoQuadro(quadroId));
    }

    public async Task SairDoQuadro(int quadroId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GrupoQuadro(quadroId));
    }

    public static string GrupoQuadro(int quadroId) => $"quadro-{quadroId}";
}