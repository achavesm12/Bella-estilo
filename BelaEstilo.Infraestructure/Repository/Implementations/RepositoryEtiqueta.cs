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
    public class RepositoryEtiqueta : IRepositoryEtiqueta
    {
        private readonly BelaEstiloContext _context;

        public RepositoryEtiqueta(BelaEstiloContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Etiqueta>> ListAsync()
        {
            var collection = await _context.Set<Etiqueta>()
            .OrderBy(c => c.Nombre)
            .AsNoTracking()
            .ToListAsync();

            return collection;
        }

        public async Task<ICollection<Etiqueta>> FindByIdsAsync(List<int> ids)
        {
            var collection = await _context.Set<Etiqueta>()
                .Where(e => ids.Contains(e.IdEtiqueta))
                .AsNoTracking()
                .ToListAsync();

            return collection;
        }

        public Task<int> AddAsync(Etiqueta etiqueta)
        {
            throw new NotImplementedException();
        }
    }
}
