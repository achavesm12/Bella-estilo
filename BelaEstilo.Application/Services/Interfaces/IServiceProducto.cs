using BelaEstilo.Application.DTOs;
using BelaEstilo.Infraestructure.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.Services.Interfaces
{
    public interface IServiceProducto
    {
        Task<ICollection<ProductoDTO>> ListAsync();

        Task<ProductoDTO> FindByIdAsync(int id);

        Task<int> AddAsync(ProductoRegistroDTO dto);

        Task UpdateAsync(ProductoRegistroDTO dto);

        Task<List<ImagenProductoDTO>> GetImagenesProductoAsync(int idProducto);

    }
}
