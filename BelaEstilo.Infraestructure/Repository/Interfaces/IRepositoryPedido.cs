using BelaEstilo.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryPedido 
    {
        Task<ICollection<Pedido>> ListAsync();

        Task<Pedido> FindByIdAsync(int id);
      
        Task<ICollection<PedidoPersonalizado>> GetPedidoPersonalizadosByPedidoIdAsync(int id);

        Task<ICollection<PedidoPersonalizadoCriterio>> GetCriteriosByPedidoIdAsync(int id);

        Task<int> AddAsync(Pedido entity);

        Task UpdateAsync(Pedido entity);

        Task<List<Pedido>> GetPedidosPorRangoFechaAsync(DateTime desde, DateTime hasta);


    }
}
