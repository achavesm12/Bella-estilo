using BelaEstilo.Infraestructure.Data;
using BelaEstilo.Infraestructure.Models;
using BelaEstilo.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Infraestructure.Repository.Implementations
{
    public class RepositoryPedido : IRepositoryPedido
    {
        private readonly BelaEstiloContext _context;

        public RepositoryPedido(BelaEstiloContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Pedido>> ListAsync()
        {
            var collection = await _context.Set<Pedido>()
            .Include(p => p.IdUsuarioNavigation) 
            .OrderByDescending(p => p.FechaPedido) 
            .AsNoTracking() 
            .ToListAsync();

            return collection;
        }

        public async Task<Pedido> FindByIdAsync(int id)
        {
            var collection = await _context.Set<Pedido>()
            .Include(p => p.IdUsuarioNavigation) 
            .Include(p => p.PedidoProducto)
                .ThenInclude(pp => pp.IdProductoNavigation) 
            .FirstOrDefaultAsync(p => p.IdPedido == id);

            return collection!;
        }

        public async Task<ICollection<PedidoPersonalizado>> GetPedidoPersonalizadosByPedidoIdAsync(int idPedido)
        {
            return await _context.Set<PedidoPersonalizado>()
                .Where(pp => pp.IdPedido == idPedido)
                .ToListAsync();
        }

        public async Task<ICollection<PedidoPersonalizadoCriterio>> GetCriteriosByPedidoIdAsync(int id)
        {
            return await _context.Set<PedidoPersonalizadoCriterio>()
                .Where(c => c.IdPedidoPersonalizadoNavigation.IdPedido == id)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> AddAsync(Pedido entity)
        {
            _context.Pedido.Add(entity);
            await _context.SaveChangesAsync();
            return entity.IdPedido; // o el campo que uses como PK
        }

        public async Task UpdateAsync(Pedido entity)
        {
            _context.Pedido.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Pedido>> GetPedidosPorRangoFechaAsync(DateTime desde, DateTime hasta)
        {
            return await _context.Pedido
                .Where(p => p.FechaPedido.Date >= desde.Date && p.FechaPedido.Date <= hasta.Date)
                .ToListAsync();
        }

    }
}
