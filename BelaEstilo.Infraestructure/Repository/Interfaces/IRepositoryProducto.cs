using BelaEstilo.Infraestructure.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryProducto
    {
        Task<ICollection<Producto>> ListAsync();

        Task<Producto> FindByIdAsync(int id);

        Task<int> AddAsync(Producto entity, List<int> idsEtiquetas, List<IFormFile> imagenes);

        Task UpdateAsync(Producto producto, List<int> idsEtiquetas, List<IFormFile> nuevasImagenes, List<int> idsImagenesAEliminar);
        
        Task<List<ImagenProducto>> GetImagenesByProductoIdAsync(int idProducto);

        Task DeleteImagenesByIdsAsync(List<int> idsImagenes);

        Task AddImagenesAsync(List<ImagenProducto> nuevasImagenes);

    }
}
