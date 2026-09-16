using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore; // Fundamental para o Include, ToListAsync e MaxAsync
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskNow.DAL.Base;
using TaskNow.DAL.Entities.Interfaces;
using TaskNow.DAO;
using TaskNow.DAO.Entities;
using TaskNow.DTO.Entities;

namespace TaskNow.DAL.Entities
{
    public class CartaoDAL(TaskNowDbContext context, IMapper mapper) : BaseDAL<Cartao, CartaoDTO>(context, mapper), ICartaoDAL
    {
        public override async Task<CartaoDTO?> GetByIdAsync(int id)
        {
            var cartao = await _context.Cartoes
                .Include(c => c.Etiquetas)
                .ThenInclude(ce => ce.Etiqueta)
                .Include(c => c.Responsavel) 
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cartao == null) return null;

            return _mapper.Map<CartaoDTO>(cartao);
        }

        public override async Task<CartaoDTO?> EditAsync(int id, CartaoDTO dto)
        {
            var cartao = await _context.Cartoes.FindAsync(id);

            if (cartao == null) return null;

            cartao.Titulo = dto.Titulo;
            cartao.Descricao = dto.Descricao;
            cartao.Prazo = dto.Prazo;
            cartao.ResponsavelId = dto.ResponsavelId;

            await _context.SaveChangesAsync();

            return _mapper.Map<CartaoDTO>(cartao);
        }

        public async Task<List<CartaoDTO>> ObterPorListaOrdenadoAsync(int listaId)
        {
            var query = _context.Cartoes
                .Where(c => c.ListaId == listaId)
                .OrderBy(c => c.Ordem)
                .ProjectTo<CartaoDTO>(_mapper.ConfigurationProvider);

            return await query.ToListAsync();
        }

        public async Task<int> ObterProximaOrdemAsync(int listaId)
        {
            var ultimaOrdem = await _context.Cartoes
                .Where(c => c.ListaId == listaId)
                .MaxAsync(c => (int?)c.Ordem);

            return (ultimaOrdem ?? 0) + 1;
        }

        public async Task<bool> CartaoPossuiEtiquetaAsync(int cartaoId, int etiquetaId)
        {
            return await _context.CartoesEtiquetas.AnyAsync(ce => ce.CartaoId == cartaoId && ce.EtiquetaId == etiquetaId);
        }

        public async Task<bool> AssociarEtiquetaAsync(int cartaoId, int etiquetaId)
        {
            var associacao = new CartaoEtiqueta
            {
                CartaoId = cartaoId,
                EtiquetaId = etiquetaId
            };

            await _context.CartoesEtiquetas.AddAsync(associacao);

            var alteracoes = await _context.SaveChangesAsync();

            return alteracoes > 0;
        }

        public async Task<bool> RemoverEtiquetaAsync(int cartaoId, int etiquetaId)
        {
            var associacao = await _context.CartoesEtiquetas
                .FirstOrDefaultAsync(ce => ce.CartaoId == cartaoId && ce.EtiquetaId == etiquetaId);

            if (associacao == null)
            {
                return false;
            }

            _context.CartoesEtiquetas.Remove(associacao);

            var alteracoes = await _context.SaveChangesAsync();

            return alteracoes > 0;
        }

        public async Task AtualizarOrdensLoteAsync(List<CartaoDTO> cartoesAtualizados)
        {
            var ids = cartoesAtualizados.Select(c => c.Id).ToList();
            var cartoesDb = await _context.Cartoes.Where(c => ids.Contains(c.Id)).ToListAsync();

            foreach (var db in cartoesDb)
            {
                var dto = cartoesAtualizados.First(c => c.Id == db.Id);
                db.Ordem = dto.Ordem;
                db.ListaId = dto.ListaId;
            }

            await _context.SaveChangesAsync();
        }
    }
}