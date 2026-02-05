using BelaEstilo.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.DTOs
{
    public record CategoriaDTO
    {
        public int IdCategoria { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public string? ImagenUrl { get; set; } 

        public virtual ICollection<Producto> Producto { get; set; } = new List<Producto>();

        public virtual ICollection<Promocion> IdPromocion { get; set; } = new List<Promocion>();

    }
}
