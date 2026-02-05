using BelaEstilo.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryEtiqueta
    {
        Task<ICollection<Etiqueta>> ListAsync();

        Task<ICollection<Etiqueta>> FindByIdsAsync(List<int> ids);

        Task<int> AddAsync(Etiqueta etiqueta);
    }
}
