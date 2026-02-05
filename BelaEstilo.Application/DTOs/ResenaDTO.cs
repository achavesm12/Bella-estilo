using BelaEstilo.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.DTOs
{
    public record ResenaDTO
    {
        public int IdResena { get; set; }

        public string? Comentario { get; set; }

        public int? Valoracion { get; set; }

        public DateTime? Fecha { get; set; }

        public int IdProducto { get; set; }

        public int IdUsuario { get; set; }

        public virtual Producto IdProductoNavigation { get; set; } = null!;

        public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
    }
}
