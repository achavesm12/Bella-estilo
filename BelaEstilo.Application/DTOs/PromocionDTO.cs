using BelaEstilo.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.DTOs
{
    public record PromocionDTO
    {
        public int IdPromocion { get; set; }

        public decimal? Descuento { get; set; }

        public DateOnly? FechaInicio { get; set; }

        public DateOnly? FechaFin { get; set; }

        public string? Nombre { get; set; }

        public virtual ICollection<Categoria> IdCategoria { get; set; } = new List<Categoria>();

        public virtual ICollection<Producto> IdProducto { get; set; } = new List<Producto>();
    }
}
