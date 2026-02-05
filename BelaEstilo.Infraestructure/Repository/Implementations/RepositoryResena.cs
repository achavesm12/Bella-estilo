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
    public class RepositoryResena : IRepositoryResena
    {
        private readonly BelaEstiloContext _context;

        public RepositoryResena(BelaEstiloContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Resena>> ListAsync()
        {
            var collection = await _context.Set<Resena>()
                .Include(x => x.IdUsuarioNavigation)
                .Include(x => x.IdProductoNavigation)
                .OrderByDescending(x => x.Fecha)
                .AsNoTracking()
                .ToListAsync();

            return collection;
        }

        public async Task<Resena> FindByIdAsync(int id)
        {
            var collection = await _context.Set<Resena>()
                .Include(x => x.IdProductoNavigation)
                .Include(x => x.IdUsuarioNavigation)
                .FirstOrDefaultAsync(x => x.IdResena == id);

            return collection!;
        }

        public async Task<int> AddAsync(Resena entity)
        {
            _context.Resena.Add(entity);
            await _context.SaveChangesAsync();
            return entity.IdResena;
        }
    }
}
