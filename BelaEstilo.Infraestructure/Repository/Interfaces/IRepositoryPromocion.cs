using BelaEstilo.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryPromocion
    {
        Task<ICollection<Promocion>> ListAsync();

        Task<Promocion> FindByIdAsync(int id);

        Task<int> AddAsync(Promocion entity);

        Task UpdateAsync(Promocion entity);
    }
}
