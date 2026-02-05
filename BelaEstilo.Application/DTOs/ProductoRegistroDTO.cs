using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.DTOs
{
    public record ProductoRegistroDTO
    {
        public int? IdProducto { get; set; } 

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; } = null!;

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string? Descripcion { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero.")]
        [Display(Name = "Precio")]
        [Required(ErrorMessage = "El precio es obligatorio.")]
        public decimal? Precio { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El campo Stock debe ser un número positivo.")]
        [Required(ErrorMessage = "La cantidad en stock es obligatorio.")]
        [Display(Name = "Stock")]
        public int? Stock { get; set; }

        [Display(Name = "Tipo de Tela")]
        [Required(ErrorMessage = "El tipo de tela es obligatorio.")]
        public string? TipoTela { get; set; }

        [Display(Name = "Talla Disponible")]
        [Required(ErrorMessage = "La talla es obligatoria.")]
        public string? TallaDisponible { get; set; }

        [Display(Name = "Activo")]
        public bool EstaActivo { get; set; } = true;

        [Required(ErrorMessage = "Debe seleccionar una categoría.")]
        [Display(Name = "Categoría")]
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "Debe seleccionar al menos una etiqueta.")]
        [Display(Name = "Etiquetas")]
        public List<int> IdsEtiquetas { get; set; } = new();

        [Required(ErrorMessage = "Debe seleccionar al menos una imagen.")]
        [Display(Name = "Imágenes")]
        public List<IFormFile> Imagenes { get; set; } = new();

        public List<ImagenProductoDTO> ImagenesActuales { get; set; } = new();
        public List<int> ImagenesAEliminar { get; set; } = new();

    }
}
