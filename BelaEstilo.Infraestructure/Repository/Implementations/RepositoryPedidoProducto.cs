using BelaEstilo.Infraestructure.Data;
using BelaEstilo.Infraestructure.Models;
using BelaEstilo.Infraestructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Infraestructure.Repository.Implementations
{
    public class RepositoryPedidoProducto : IRepositoryPedidoProducto
    {
        private readonly BelaEstiloContext _context;

        public RepositoryPedidoProducto(BelaEstiloContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PedidoProducto entity)
        {

            _context.PedidoProducto.Add(entity);
            await _context.SaveChangesAsync();
        }
    }
}
