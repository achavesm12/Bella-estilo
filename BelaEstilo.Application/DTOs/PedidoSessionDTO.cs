using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.DTOs
{
    public record PedidoSessionDTO
    {

        public List<CarritoItemDTO> Items { get; set; } = new();

        public List<CarritoItemPersonalizadoDTO> ItemsPersonalizados { get; set; } = new();
       
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }

        public class CarritoItemPersonalizadoDTO
        {
            public Guid LineId { get; set; } = Guid.NewGuid(); // para identificar la línea
            public int IdProducto { get; set; }
            public string NombreProducto { get; set; } = null!;
            public decimal CostoBase { get; set; }
            public List<SeleccionCriterioDTO> Criterios { get; set; } = new();
            public int Cantidad { get; set; } = 1;
            public decimal TotalLinea { get; set; } // con IVA incluido (si así lo decides)
        }

        public class SeleccionCriterioDTO
        {
            public string NombreCriterio { get; set; } = null!;
            public string OpcionSeleccionada { get; set; } = "Sí"; // para este flujo de checkbox
            public decimal CostoExtra { get; set; }
        }

        //Productos normales
        public class CarritoItemDTO
        {
            public Guid LineId { get; set; } = Guid.NewGuid();
            public int IdProducto { get; set; }
            public string NombreProducto { get; set; } = null!;
            public decimal PrecioUnitario { get; set; }
            public int Cantidad { get; set; } = 1;
            public decimal TotalLinea { get; set; }
        }
    }
}
