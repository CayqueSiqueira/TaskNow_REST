using Microsoft.EntityFrameworkCore;
using TaskNow.DAL.Base;
using TaskNow.DAL.Entities.Interfaces;
using TaskNow.DAO;
using TaskNow.DAO.Entities;
using TaskNow.DTO.Entities;

namespace TaskNow.DAL.Entities;

public class ListaDAL(TaskNowDbContext context) : BaseDAL<Lista, ListaDTO>(context), IListaDAL
{
    public override async Task<ListaDTO?> GetByIdAsync(int id)
    {
        var entity = await GetQuery(true, x => x.Cartoes).FirstOrDefaultAsync(x => x.Id == id);
        return entity is null ? null : Map(entity);
    }

    public override async Task<ListaDTO> CreateAsync(ListaDTO dto)
    {
        var entity = new Lista
        {
            QuadroId = dto.QuadroId,
            Nome = dto.Nome,
            Ordem = dto.Ordem
        };

        DbSet.Add(entity);
        await Context.SaveChangesAsync();
        return Map(entity);
    }

    public override async Task<ListaDTO?> EditAsync(int id, ListaDTO dto)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity is null)
        {
            return null;
        }

        entity.Nome = dto.Nome;
        await Context.SaveChangesAsync();
        return Map(entity);
    }

    public async Task<List<ListaDTO>> ObterPorQuadroOrdenadoAsync(int quadroId)
    {
        return await GetQuery(true, x => x.Cartoes)
            .Where(x => x.QuadroId == quadroId)
            .OrderBy(x => x.Ordem)
            .Select(x => Map(x))
            .ToListAsync();
    }

    public async Task<int> ObterProximaOrdemAsync(int quadroId)
    {
        var ultimaOrdem = await GetQuery(true)
            .Where(x => x.QuadroId == quadroId)
            .Select(x => (int?)x.Ordem)
            .MaxAsync();

        return (ultimaOrdem ?? 0) + 1;
    }

    private static ListaDTO Map(Lista entity) => new()
    {
        Id = entity.Id,
        QuadroId = entity.QuadroId,
        Nome = entity.Nome,
        Ordem = entity.Ordem,
        Cartoes = entity.Cartoes
            .OrderBy(x => x.Ordem)
            .Select(x => new CartaoDTO
            {
                Id = x.Id,
                ListaId = x.ListaId,
                Titulo = x.Titulo,
                Descricao = x.Descricao,
                Ordem = x.Ordem,
                Prazo = x.Prazo,
                ResponsavelId = x.ResponsavelId
            })
            .ToList()
    };
}