using BelaEstilo.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.DTOs
{
    public record ProductoPersonalizacionOpcionDTO
    {
        public int IdOpcion { get; set; }

        public int IdProducto { get; set; }

        public string NombreCriterio { get; set; } = null!;

        public decimal CostoExtra { get; set; }

        public bool Activo { get; set; }

    }
}
