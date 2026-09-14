using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskNow.DAL.Base;
using TaskNow.DAL.Entities.Interfaces;
using TaskNow.DAO;
using TaskNow.DAO.Entities;
using TaskNow.DTO.Entities;

namespace TaskNow.DAL.Entities;

public class AtividadeCartaoDAL(TaskNowDbContext context, IMapper mapper) : BaseDAL<AtividadeCartao, AtividadeCartaoDTO>(context, mapper), IAtividadeCartaoDAL
{
    public async Task<List<AtividadeCartaoDTO>> ObterPorCartaoOrdenadoAsync(int cartaoId)
    {
        var atividades = await GetQuery(true, a => a.Usuario)
            .Where(a => a.CartaoId == cartaoId)
            .OrderByDescending(a => a.CriadoEm)
            .ToListAsync();

        return _mapper.Map<List<AtividadeCartaoDTO>>(atividades);
    }
}
