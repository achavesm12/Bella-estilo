using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.DTOs
{
    public class PedidoPersonalizadoDTO
    {
        public int IdPedidoPersonalizado { get; set; }

        public int IdPedido { get; set; }

        public int IdProducto { get; set; }

        public string NombreProductoPersonalizado { get; set; } = null!;

        public decimal CostoBase { get; set; }

        public decimal TotalProductoPersonalizado { get; set; }

        //public ICollection<PedidoPersonalizadoCriterioDTO> Criterios { get; set; } = new List<PedidoPersonalizadoCriterioDTO>();

        public List<PedidoPersonalizadoCriterioDTO> Criterios { get; set; } = new();

        public List<string> CriteriosSeleccionados { get; set; } = new();

        public List<int> CriteriosSeleccionadosIds { get; set; } = new();

    }
}
