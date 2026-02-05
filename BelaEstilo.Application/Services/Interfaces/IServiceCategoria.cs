using BelaEstilo.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.Services.Interfaces
{
    public interface IServiceCategoria
    {

        Task<ICollection<CategoriaDTO>> ListAsync();
    }
}
