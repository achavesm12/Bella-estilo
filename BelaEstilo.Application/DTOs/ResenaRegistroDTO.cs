using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.DTOs
{
    public record ResenaRegistroDTO
    {
        [Required(ErrorMessage = "El comentario es obligatorio.")]
        [Display(Name = "Comentario")]
        public string Comentario { get; set; } = null!;

        [Required(ErrorMessage = "La valoración es obligatoria.")]
        [Range(1, 5, ErrorMessage = "La valoración debe estar entre 1 y 5.")]
        [Display(Name = "Valoración")]
        public int Valoracion { get; set; }

        [Required]
        public int IdProducto { get; set; }
    }
}
