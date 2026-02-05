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
    public class RepositoryPromocion : IRepositoryPromocion
    {
        private readonly BelaEstiloContext _context;

        public RepositoryPromocion(BelaEstiloContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Promocion>> ListAsync()
        {
            var collection = await _context.Set<Promocion>()
                .Include(x => x.IdProducto)
                .Include(x => x.IdCategoria)
                    .OrderByDescending(x => x.FechaInicio)
                    .AsNoTracking()
                    .ToListAsync();

            return collection;
        }

        public async Task<Promocion?> FindByIdAsync(int id)
        {
            var collection = await _context.Promocion
                .Include(p => p.IdProducto)
                .Include(p => p.IdCategoria)
                .FirstOrDefaultAsync(p => p.IdPromocion == id);

            return collection;
        }

        public async Task<int> AddAsync(Promocion entity)
        {
            if (entity.IdCategoria != null)
            {
                foreach (var categoria in entity.IdCategoria)
                {
                    _context.Categoria.Attach(categoria);
                }
            }

            if (entity.IdProducto != null)
            {
                foreach (var producto in entity.IdProducto)
                {
                    _context.Producto.Attach(producto);
                }
            }
            _context.Promocion.Add(entity);
            await _context.SaveChangesAsync();
            return entity.IdPromocion;
        }

        public async Task UpdateAsync(Promocion entity)
        {
            var promocionExistente = await _context.Promocion
                .Include(p => p.IdCategoria)
                .Include(p => p.IdProducto)
                .FirstOrDefaultAsync(p => p.IdPromocion == entity.IdPromocion);

            if (promocionExistente == null)
                throw new Exception("Promoción no encontrada.");

            // Validar que la promoción no haya iniciado
            if (promocionExistente.FechaInicio < DateOnly.FromDateTime(DateTime.Today))
                throw new Exception("No se puede modificar una promoción que ya ha iniciado.");

            // Actualizar campos simples
            promocionExistente.Nombre = entity.Nombre;
            promocionExistente.Descuento = entity.Descuento;
            promocionExistente.FechaInicio = entity.FechaInicio;
            promocionExistente.FechaFin = entity.FechaFin;

            // Actualizar relaciones
            promocionExistente.IdCategoria.Clear();
            promocionExistente.IdProducto.Clear();

            if (entity.IdCategoria?.Any() == true)
            {
                foreach (var cat in entity.IdCategoria)
                {
                    _context.Entry(cat).State = EntityState.Unchanged;
                    promocionExistente.IdCategoria.Add(cat);
                }
            }

            if (entity.IdProducto?.Any() == true)
            {
                foreach (var prod in entity.IdProducto)
                {
                    _context.Entry(prod).State = EntityState.Unchanged;
                    promocionExistente.IdProducto.Add(prod);
                }
            }

            await _context.SaveChangesAsync();
        }

    }
}
