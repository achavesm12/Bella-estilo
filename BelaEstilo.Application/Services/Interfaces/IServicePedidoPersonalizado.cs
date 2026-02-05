using BelaEstilo.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.Services.Interfaces
{
    public interface IServicePedidoPersonalizado
    {
        Task<PedidoPersonalizadoDTO> GetPedidoBaseAsync(int id);

        Task<int> AddPersonalizadoAsync(PedidoDTO pedidoDto, PedidoPersonalizadoDTO personalizadoDTO);

        Task<int> CrearPedidoVacioAsync(PedidoDTO pedidoDto);                    
        
        Task AddPersonalizadoEnPedidoExistenteAsync(int idPedido, PedidoPersonalizadoDTO dto); 

    }
}
