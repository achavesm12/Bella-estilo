using BelaEstilo.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryPedidoPersonalizado
    {
        Task <Producto?> GetProductoBaseAsync(int idProducto);

        Task <int> AddPedidoPersonalizadoAsync (PedidoPersonalizado entity);

        Task AddCriterioAsync(PedidoPersonalizadoCriterio entity);

        Task<List<ProductoPersonalizacionOpcion>> GetOpcionesPorProductoAsync(int idProducto);
    }
}
