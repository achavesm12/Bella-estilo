using BelaEstilo.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryResena
    {
        Task<ICollection<Resena>> ListAsync();

        Task<Resena> FindByIdAsync(int id);

        Task<int> AddAsync(Resena entity);

    }
}
