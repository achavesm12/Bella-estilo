using BelaEstilo.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.Services.Interfaces
{
    public interface IServiceResena
    {
        Task<ICollection<ResenaDTO>> ListAsync();

        Task<ResenaDTO> FindByIdAsync(int id);

        Task<int> AddAsync(ResenaRegistroDTO dto, int idUsuario);

    }
}
