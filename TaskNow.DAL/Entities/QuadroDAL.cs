using Microsoft.EntityFrameworkCore;
using TaskNow.DAL.Base;
using TaskNow.DAL.Entities.Interfaces;
using TaskNow.DAO;
using TaskNow.DAO.Entities;
using TaskNow.DTO.Entities;

namespace TaskNow.DAL.Entities;

public class QuadroDAL(TaskNowDbContext context) : BaseDAL<Quadro, QuadroDTO>(context), IQuadroDAL
{
    public override async Task<QuadroDTO?> GetByIdAsync(int id)
    {
        var entity = await GetQuery(true).FirstOrDefaultAsync(x => x.Id == id);
        return entity is null ? null : Map(entity);
    }

    public override async Task<QuadroDTO> CreateAsync(QuadroDTO dto)
    {
        var entity = new Quadro
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            DonoId = dto.DonoId,
            CriadoEm = DateTime.UtcNow,
            Membros =
            [
                new MembroQuadro
                {
                    UsuarioId = dto.DonoId,
                    Papel = PapelMembro.Dono
                }
            ]
        };

        DbSet.Add(entity);
        await Context.SaveChangesAsync();
        return Map(entity);
    }

    public override async Task<QuadroDTO?> EditAsync(int id, QuadroDTO dto)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity is null)
        {
            return null;
        }

        entity.Nome = dto.Nome;
        entity.Descricao = dto.Descricao;
        await Context.SaveChangesAsync();
        return Map(entity);
    }

    public async Task<List<QuadroDTO>> ObterPorUsuarioAsync(string usuarioId)
    {
        return await GetQuery(true)
            .Where(x => x.DonoId == usuarioId || x.Membros.Any(m => m.UsuarioId == usuarioId))
            .OrderBy(x => x.Nome)
            .Select(x => Map(x))
            .ToListAsync();
    }

    public async Task<bool> UsuarioTemAcessoAsync(int quadroId, string usuarioId)
    {
        return await GetQuery(true)
            .AnyAsync(x => x.Id == quadroId && (x.DonoId == usuarioId || x.Membros.Any(m => m.UsuarioId == usuarioId)));
    }

    private static QuadroDTO Map(Quadro entity) => new()
    {
        Id = entity.Id,
        Nome = entity.Nome,
        Descricao = entity.Descricao,
        DonoId = entity.DonoId,
        CriadoEm = entity.CriadoEm
    };
}