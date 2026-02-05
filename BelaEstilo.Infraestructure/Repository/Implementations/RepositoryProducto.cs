using BelaEstilo.Infraestructure.Data;
using BelaEstilo.Infraestructure.Models;
using BelaEstilo.Infraestructure.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Infraestructure.Repository.Implementations
{
    public class RepositoryProducto : IRepositoryProducto
    {
        private readonly BelaEstiloContext _context;

        public RepositoryProducto(BelaEstiloContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Producto>> ListAsync()
        {
            var collection = await _context.Set<Producto>()
                .Include(x => x.IdCategoriaNavigation)
                .OrderByDescending(x => x.IdProducto)
                .AsNoTracking()
                .ToListAsync();
            
            return collection;
        }

        public async Task<Producto> FindByIdAsync(int id)
        {
            var @object = await _context.Set<Producto>()
               .Include(x => x.IdCategoriaNavigation)
               .Include(x => x.ImagenProducto)
               .Include(x => x.IdEtiqueta)
               .Include(x => x.Resena)
               .ThenInclude(x => x.IdUsuarioNavigation)
               .FirstOrDefaultAsync(x => x.IdProducto == id);

            return @object;
        }

        public async Task<int> AddAsync(Producto entity, List<int> idsEtiquetas, List<IFormFile> imagenes)
        {
            // Cargar etiquetas de la BD
            var etiquetas = await _context.Etiqueta
                .Where(e => idsEtiquetas.Contains(e.IdEtiqueta))
                .ToListAsync();
            entity.IdEtiqueta = etiquetas;

            // Procesar y agregar las imágenes
            foreach (var imagen in imagenes)
            {
                if (imagen.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await imagen.CopyToAsync(ms);

                    var imagenProducto = new ImagenProducto
                    {
                        Imagen = ms.ToArray(),
                        IdProductoNavigation = entity 
                    };
                    entity.ImagenProducto.Add(imagenProducto);
                }
            }

            _context.Producto.Add(entity);
            await _context.SaveChangesAsync();

            return entity.IdProducto;
        }

        public async Task UpdateAsync(Producto producto, List<int> idsEtiquetas, List<IFormFile> nuevasImagenes, List<int> idsImagenesAEliminar)
        {
            var productoExistente = await _context.Producto
                .Include(p => p.ImagenProducto)
                .Include(p => p.IdEtiqueta)
                .FirstOrDefaultAsync(p => p.IdProducto == producto.IdProducto);

            if (productoExistente == null)
                throw new Exception("Producto no encontrado");

            // Actualizar propiedades básicas
            _context.Entry(productoExistente).CurrentValues.SetValues(producto);

            // Actualizar etiquetas (muchos a muchos)
            if (idsEtiquetas != null && idsEtiquetas.Any())
            {
                productoExistente.IdEtiqueta.Clear();
                var etiquetas = await _context.Etiqueta.Where(e => idsEtiquetas.Contains(e.IdEtiqueta)).ToListAsync();
                foreach (var etiqueta in etiquetas)
                {
                    productoExistente.IdEtiqueta.Add(etiqueta);
                }
            }

            // Eliminar imágenes seleccionadas
            if (idsImagenesAEliminar != null && idsImagenesAEliminar.Any())
            {
                var imagenesEliminar = productoExistente.ImagenProducto
                    .Where(i => idsImagenesAEliminar.Contains(i.IdImagen))
                    .ToList();

                _context.ImagenProducto.RemoveRange(imagenesEliminar);
            }

            // Agregar nuevas imágenes
            if (nuevasImagenes != null && nuevasImagenes.Any())
            {
                foreach (var formFile in nuevasImagenes)
                {
                    using var ms = new MemoryStream();
                    await formFile.CopyToAsync(ms);
                    var imagen = new ImagenProducto
                    {
                        Imagen = ms.ToArray(),
                        IdProducto = producto.IdProducto
                    };
                    productoExistente.ImagenProducto.Add(imagen);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<ImagenProducto>> GetImagenesByProductoIdAsync(int idProducto)
        {
            return await _context.ImagenProducto
                .Where(i => i.IdProducto == idProducto)
                .ToListAsync();
        }

        public async Task DeleteImagenesByIdsAsync(List<int> idsImagenes)
        {
            var imagenes = await _context.ImagenProducto
                                 .Where(i => idsImagenes.Contains(i.IdImagen))
                                 .ToListAsync();

            _context.ImagenProducto.RemoveRange(imagenes);
            await _context.SaveChangesAsync();
        }

        public async Task AddImagenesAsync(List<ImagenProducto> nuevasImagenes)
        {
            await _context.ImagenProducto.AddRangeAsync(nuevasImagenes);
            await _context.SaveChangesAsync();
        }

    }
}