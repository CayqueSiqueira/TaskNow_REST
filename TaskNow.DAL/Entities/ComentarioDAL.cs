using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskNow.DAL.Base;
using TaskNow.DAL.Entities.Interfaces;
using TaskNow.DAO;
using TaskNow.DAO.Entities;
using TaskNow.DTO.Entities;

namespace TaskNow.DAL.Entities;

public class ComentarioDAL(TaskNowDbContext context, IMapper mapper) : BaseDAL<Comentario, ComentarioDTO>(context, mapper), IComentarioDAL
{
    public async Task<List<ComentarioDTO>> ObterPorCartaoAsync(int cartaoId)
    {
        var entidades = await GetQuery(true, c => c.Autor)
            .Where(c => c.CartaoId == cartaoId)
            .OrderBy(c => c.CriadoEm)
            .ToListAsync();

        return _mapper.Map<List<ComentarioDTO>>(entidades);
    }
}
