using BelaEstilo.Application.DTOs;
using BelaEstilo.Application.Services.Interfaces;
using BelaEstilo.Infraestructure.Models;
using BelaEstilo.Infraestructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.Services.Implementations
{
    public class ServicePedidoProducto : IServicePedidoProducto
    {
        private readonly IRepositoryPedidoProducto _repository;

        public ServicePedidoProducto(IRepositoryPedidoProducto repository)
        {
            _repository = repository;
        }

        public async Task AddAsync(PedidoProductoDTO dto)
        {
            var entity = new PedidoProducto
            {
                IdPedido = dto.IdPedido,
                IdProducto = dto.IdProducto,
                Cantidad = dto.Cantidad,
                PrecioUnitario = dto.PrecioUnitario
            };

            await _repository.AddAsync(entity);
        }
    }
}
