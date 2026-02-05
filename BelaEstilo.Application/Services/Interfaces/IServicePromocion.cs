using BelaEstilo.Application.DTOs;
using BelaEstilo.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.Services.Interfaces
{
    public interface IServicePromocion
    {
        Task<ICollection<PromocionDTO>> ListAsync();

        Task<PromocionDTO> FindByIdAsync(int id);

        Task<int> AddAsync(PromocionRegistroDTO dto);

        Task UpdateAsync(PromocionRegistroDTO dto);

        Task<List<Promocion>> ListEntityAsync();


    }
}
