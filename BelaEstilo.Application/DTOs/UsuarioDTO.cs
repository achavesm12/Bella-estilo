using BelaEstilo.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.DTOs
{
    public record UsuarioDTO
        //DTO para Lista y detalle de usuarios 
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]

        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ser un correo válido")]
        public string Correo { get; set; } = null!;

        [Required(ErrorMessage = "El rol es obligatorio")]
        public string Rol { get; set; } = null!;
    }
}
