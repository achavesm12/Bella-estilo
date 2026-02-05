using BelaEstilo.Application.DTOs;
using BelaEstilo.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.Mappers
{
    public static class PedidoPersonalizadoMapper
    {
        // Entidad Producto -> DTO inicial para la pantalla
        public static PedidoPersonalizadoDTO ToPersonalizadoDTO(Producto p)
            => new()
            {
                IdProducto = p.IdProducto,
                NombreProductoPersonalizado = p.Nombre,
                CostoBase = (decimal)p.Precio,
                TotalProductoPersonalizado = (decimal)p.Precio
            };

        public static Pedido ToEntity(this PedidoDTO d)
        => new()
        {
            IdPedido = d.IdPedido,
            FechaPedido = d.FechaPedido,
            Estado = d.Estado,
            Total = d.Total,
            IdUsuario = d.IdUsuario,
            MetodoPago = d.MetodoPago,
            DireccionEnvio = d.DireccionEnvio,        
        };

        public static PedidoPersonalizado ToEntity(this PedidoPersonalizadoDTO d)
       => new()
       {
           IdPedidoPersonalizado = d.IdPedidoPersonalizado,
           IdPedido = d.IdPedido,
           IdProducto = d.IdProducto,
           NombreProductoPersonalizado = d.NombreProductoPersonalizado,
           CostoBase = d.CostoBase,
           TotalProductoPersonalizado = d.TotalProductoPersonalizado
       };

        public static PedidoPersonalizadoCriterio ToEntity(this PedidoPersonalizadoCriterioDTO d)
        => new()
        {
            Id = d.Id,
            IdPedidoPersonalizado = d.IdPedidoPersonalizado,
            NombreCriterio = d.NombreCriterio,
            OpcionSeleccionada = d.OpcionSeleccionada,
            CostoExtra = d.CostoExtra
        };
    }
}
