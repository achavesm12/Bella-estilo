using BelaEstilo.Application.DTOs;
using BelaEstilo.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.Mappers
{
    public static class PedidoMapper
    {
        public static PedidoDTO MapToDTO (Pedido pedido)
        {
            return new PedidoDTO
            {
                IdPedido = pedido.IdPedido,
                FechaPedido = pedido.FechaPedido,
                Estado = pedido.Estado,
                Total = pedido.Total,
                IdUsuario = pedido.IdUsuario,
                IdUsuarioNavigation = pedido.IdUsuarioNavigation,
                MetodoPago = pedido.MetodoPago,
                DireccionEnvio = pedido.DireccionEnvio,
                PedidoProducto = pedido.PedidoProducto.Select(pp => new PedidoProductoDTO
                {
                    IdPedido = pp.IdPedido,
                    IdProducto = pp.IdProducto,
                    Cantidad = pp.Cantidad,
                    PrecioUnitario = pp.PrecioUnitario,
                    IdProductoNavigation = pp.IdProductoNavigation,
                    IdPedidoNavigation = null! // O null si no lo necesitás
                }).ToList(),

            };

        }
    }
}
