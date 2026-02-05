using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.DTOs
{
    public record ImagenProductoDTO
    {
        public int IdImagen { get; set; }
        public byte[] Imagen { get; set; } = null!;
        public bool Eliminar { get; set; } = false;
    }
}
