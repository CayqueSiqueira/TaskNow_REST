using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TaskNow.DAL.Base;
using TaskNow.DAL.Entities.Interfaces;
using TaskNow.DAO;
using TaskNow.DAO.Entities;
using TaskNow.DTO.Entities;

namespace TaskNow.DAL.Entities;

public class MembroQuadroDAL(TaskNowDbContext context, IMapper mapper) : BaseDAL<MembroQuadro, MembroQuadroDTO>(context, mapper), IMembroQuadroDAL
{
    public async Task<List<MembroQuadroDTO>> ObterPorQuadroAsync(int quadroId)
    {
        return await _context.MembrosQuadro
            .Where(m => m.QuadroId == quadroId)
            .ProjectTo<MembroQuadroDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<MembroQuadroDTO?> ObterPorUsuarioEQuadroAsync(int quadroId, string usuarioId)
    {
        var membro = await _context.MembrosQuadro
            .FirstOrDefaultAsync(m => m.QuadroId == quadroId && m.UsuarioId == usuarioId);
            
        if (membro == null) return null;
        return _mapper.Map<MembroQuadroDTO>(membro);
    }

    public async Task<bool> EhDonoAsync(int quadroId, string usuarioId)
    {
        return await _context.MembrosQuadro
            .AnyAsync(m => m.QuadroId == quadroId && m.UsuarioId == usuarioId && m.Papel == PapelMembro.Dono);
    }

    public async Task<bool> EhMembroAsync(int quadroId, string usuarioId)
    {
        return await _context.MembrosQuadro
            .AnyAsync(m => m.QuadroId == quadroId && m.UsuarioId == usuarioId);
    }

    public async Task<bool> RemoverMembroAsync(int quadroId, string usuarioId)
    {
        var membro = await _context.MembrosQuadro
            .FirstOrDefaultAsync(m => m.QuadroId == quadroId && m.UsuarioId == usuarioId);

        if (membro == null) return false;

        _context.MembrosQuadro.Remove(membro);
        return await _context.SaveChangesAsync() > 0;
    }
}
