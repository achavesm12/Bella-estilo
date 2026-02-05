using BelaEstilo.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.DTOs
{
    public record PedidoProductoDTO
    {
        public int IdPedido { get; set; }

        public int IdProducto { get; set; }

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public virtual Pedido IdPedidoNavigation { get; set; } = null!;

        public virtual Producto IdProductoNavigation { get; set; } = null!;
    }
}
