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
    public class RepositoryPedidoPersonalizado : IRepositoryPedidoPersonalizado
    {
        private readonly BelaEstiloContext _context;

        public RepositoryPedidoPersonalizado(BelaEstiloContext context)
        {
            _context = context;
        }

        public async Task AddCriterioAsync(PedidoPersonalizadoCriterio entity)
        {
            await _context.PedidoPersonalizadoCriterio.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<int> AddPedidoPersonalizadoAsync(PedidoPersonalizado entity)
        {
            await _context.PedidoPersonalizado.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity.IdPedidoPersonalizado;
        }

        public async Task<Producto?> GetProductoBaseAsync(int idProducto)
        =>
            await _context.Producto.AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdProducto == idProducto);

        public async Task<List<ProductoPersonalizacionOpcion>> GetOpcionesPorProductoAsync(int idProducto)
        => await _context.ProductoPersonalizacionOpcion 
                 .AsNoTracking()
                 .Where(o => o.IdProducto == idProducto && o.Activo)
                 .OrderBy(o => o.IdOpcion)
                 .ToListAsync();

    }


}
