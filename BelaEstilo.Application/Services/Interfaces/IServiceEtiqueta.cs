using BelaEstilo.Application.DTOs;
using BelaEstilo.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.Services.Interfaces
{
    public interface IServiceEtiqueta
    {
        Task<ICollection<EtiquetaDTO>> ListAsync();
        Task<ICollection<EtiquetaDTO>> FindByIdsAsync(List<int> ids);
    }
}
