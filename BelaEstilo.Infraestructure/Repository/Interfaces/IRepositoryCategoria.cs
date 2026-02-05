using BelaEstilo.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryCategoria
    {

        Task<ICollection<Categoria>> ListAsync();
    }
}
