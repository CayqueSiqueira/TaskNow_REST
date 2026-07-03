namespace TaskNow.DAL.Entities.Interfaces;

public interface IMembroQuadroDAL
{
    Task<bool> EhMembroAsync(int quadroId, string usuarioId);
}