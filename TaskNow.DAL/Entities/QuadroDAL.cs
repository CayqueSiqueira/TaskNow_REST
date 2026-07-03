using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskNow.DAL.Base;
using TaskNow.DAL.Entities.Interfaces;
using TaskNow.DAO;
using TaskNow.DAO.Entities;
using TaskNow.DTO.Entities;

namespace TaskNow.DAL.Entities;

public class QuadroDAL(TaskNowDbContext context, IMapper mapper) : BaseDAL<Quadro, QuadroDTO>(context, mapper), IQuadroDAL
{
    public override async Task<QuadroDTO?> GetByIdAsync(int id)
    {
        var entity = await GetQuery(true).FirstOrDefaultAsync(x => x.Id == id);
        return entity is null ? null : _mapper.Map<QuadroDTO>(entity);
    }

    public override async Task<QuadroDTO> CreateAsync(QuadroDTO dto)
    {
        var entity = _mapper.Map<Quadro>(dto);
        entity.CriadoEm = DateTime.UtcNow;
        entity.Membros =
        [
            new MembroQuadro
            {
                UsuarioId = dto.DonoId,
                Papel = PapelMembro.Dono
            }
        ];

        _dbSet.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.Map<QuadroDTO>(entity);
    }

    public override async Task<QuadroDTO?> EditAsync(int id, QuadroDTO dto)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity is null)
        {
            return null;
        }

        entity.Nome = dto.Nome;
        entity.Descricao = dto.Descricao;
        await _context.SaveChangesAsync();
        return _mapper.Map<QuadroDTO>(entity);
    }

    public async Task<List<QuadroDTO>> ObterPorUsuarioAsync(string usuarioId)
    {
        var entities = await GetQuery(true)
            .Where(x => x.DonoId == usuarioId || x.Membros.Any(m => m.UsuarioId == usuarioId))
            .OrderBy(x => x.Nome)
            .ToListAsync();

        return _mapper.Map<List<QuadroDTO>>(entities);
    }

    public async Task<bool> UsuarioTemAcessoAsync(int quadroId, string usuarioId)
    {
        return await GetQuery(true)
            .AnyAsync(x => x.Id == quadroId && (x.DonoId == usuarioId || x.Membros.Any(m => m.UsuarioId == usuarioId)));
    }
}
