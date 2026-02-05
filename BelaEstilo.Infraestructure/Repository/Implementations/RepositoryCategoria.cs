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
    public class RepositoryCategoria : IRepositoryCategoria
    {

        private readonly BelaEstiloContext _context;

        public RepositoryCategoria(BelaEstiloContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Categoria>> ListAsync()
        {
            var collection = await _context.Set<Categoria>()
            .OrderBy(c => c.Nombre) 
            .AsNoTracking()
            .ToListAsync();

            return collection;
        }
    }
}
