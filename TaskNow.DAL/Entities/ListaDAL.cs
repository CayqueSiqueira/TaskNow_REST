using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskNow.DAL.Base;
using TaskNow.DAL.Entities.Interfaces;
using TaskNow.DAO;
using TaskNow.DAO.Entities;
using TaskNow.DTO.Entities;

namespace TaskNow.DAL.Entities;

public class ListaDAL(TaskNowDbContext context, IMapper mapper) : BaseDAL<Lista, ListaDTO>(context, mapper), IListaDAL
{
    public override async Task<ListaDTO?> GetByIdAsync(int id)
    {
        var entity = await GetQuery(true, x => x.Cartoes).FirstOrDefaultAsync(x => x.Id == id);
        return entity is null ? null : _mapper.Map<ListaDTO>(entity);
    }

    public override async Task<ListaDTO?> EditAsync(int id, ListaDTO dto)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity is null)
        {
            return null;
        }

        entity.Nome = dto.Nome;
        await _context.SaveChangesAsync();
        return _mapper.Map<ListaDTO>(entity);
    }

    public async Task<List<ListaDTO>> ObterPorQuadroOrdenadoAsync(int quadroId)
    {
        var entities = await GetQuery(true, x => x.Cartoes)
            .Where(x => x.QuadroId == quadroId)
            .OrderBy(x => x.Ordem)
            .ToListAsync();

        return _mapper.Map<List<ListaDTO>>(entities);
    }

    public async Task<int> ObterProximaOrdemAsync(int quadroId)
    {
        var ultimaOrdem = await GetQuery(true)
            .Where(x => x.QuadroId == quadroId)
            .Select(x => (int?)x.Ordem)
            .MaxAsync();

        return (ultimaOrdem ?? 0) + 1;
    }

    public async Task AtualizarOrdensAsync(Dictionary<int, int> ordens)
    {
        foreach (var (listaId, ordem) in ordens)
        {
            var lista = await _dbSet.FindAsync(listaId);
            if (lista is not null)
            {
                lista.Ordem = ordem;
            }
        }
        await _context.SaveChangesAsync();
    }
}
