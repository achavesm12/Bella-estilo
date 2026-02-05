using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.DTOs
{
    public record UsuarioRegistroDTO
        //DTO para registrar usuarios
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ser un correo válido")]
        public string Correo { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es obligatoria")]   
        [StringLength(14, MinimumLength = 6, ErrorMessage = "Debe tener entre 6 y 14 caracteres")]
        public string Contrasenna { get; set; } = null!;

        [Required(ErrorMessage = "La confirmación de contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Compare("Contrasenna", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContrasenna { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio")]
        public string Rol { get; set; } = "Cliente";
    }
}
