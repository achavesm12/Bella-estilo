using BelaEstilo.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.DTOs
{
    public record PedidoDTO
    {
        public int IdPedido { get; set; }

        public DateTime FechaPedido { get; set; }

        public string? Estado { get; set; }

        public decimal? Total { get; set; }

        public int IdUsuario { get; set; }

        public string? MetodoPago { get; set; }

        public string? DireccionEnvio { get; set; }
        
        public decimal Subtotal { get; set; }
        
        public decimal IVA { get; set; }

        public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

        public virtual ICollection<PedidoProductoDTO> PedidoProducto { get; set; } = new List<PedidoProductoDTO>();
                
        public virtual ICollection<PedidoPersonalizadoDTO> PedidoPersonalizado { get; set; } = new List<PedidoPersonalizadoDTO>();

        //Colección de DTO PedidoProducto para el mapeo
        //public virtual ICollection<PedidoProductoDTO> PedidoProductoDTO { get; set; } = new List<PedidoProductoDTO>();

    }
}
