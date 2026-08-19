using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskNow.DAL.Base;
using TaskNow.DAL.Entities.Interfaces;
using TaskNow.DAO;
using TaskNow.DAO.Entities;
using TaskNow.DTO.Entities;

namespace TaskNow.DAL.Entities;

public class EtiquetaDAL(TaskNowDbContext context, IMapper mapper) : BaseDAL<Etiqueta, EtiquetaDTO>(context, mapper), IEtiquetaDAL
{
    public async Task<List<EtiquetaDTO>> ObterPorQuadroAsync(int quadroId)
    {
        var entities = await GetQuery(true)
            .Where(x => x.QuadroId == quadroId)
            .ToListAsync();

        return _mapper.Map<List<EtiquetaDTO>>(entities);
    }

    public async Task<bool> ExisteNomeNoQuadroAsync(int quadroId, string nome, int? etiquetaIgnoradaId = null)
    {
        var query = GetQuery(true)
            .Where(x => x.QuadroId == quadroId && x.Nome.ToLower() == nome.ToLower());

        if (etiquetaIgnoradaId.HasValue)
        {
            query = query.Where(x => x.Id != etiquetaIgnoradaId.Value);
        }

        return await query.AnyAsync();
    }

    public override async Task<EtiquetaDTO?> EditAsync(int id, EtiquetaDTO dto)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity is null)
        {
            return null;
        }

        entity.Nome = dto.Nome;
        entity.Cor = dto.Cor;
        
        await _context.SaveChangesAsync();
        return _mapper.Map<EtiquetaDTO>(entity);
    }
}
