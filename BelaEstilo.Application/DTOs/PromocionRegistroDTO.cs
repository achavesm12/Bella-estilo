using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.DTOs
{
    public record PromocionRegistroDTO
    {
        public int IdPromocion { get; set; }
        
        [Required(ErrorMessage = "El nombre de la promoción es obligatorio")]
        [Display(Name = "Nombre de la promoción")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El tipo de promoción es obligatorio")]
        [Display(Name = "Tipo de promoción")]
        public string TipoPromocion { get; set; } = null!;

        [Display(Name = "Categoría")]
        public List<int>? IdCategoria { get; set; }

        [Display(Name = "Producto")]
        public List<int>? IdProducto { get; set; }

        [Required(ErrorMessage = "El descuento es obligatorio")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Solo se permiten números en el campo de descuento.")]
        [Range(0, 100, ErrorMessage = "Ingrese un valor válido para el descuento")]
        [Display(Name = "Descuento (porcentaje o cantidad)")]
        public decimal Descuento { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de inicio")]
        public DateOnly FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de fin")]
        public DateOnly FechaFin { get; set; }
    }
}
