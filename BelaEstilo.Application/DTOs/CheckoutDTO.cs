using BelaEstilo.Application.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static BelaEstilo.Application.DTOs.PedidoSessionDTO;

namespace BelaEstilo.Application.DTOs
{
    public class CheckoutDTO
    {
        // Encabezado
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Debe seleccionar un usuario.")]
        public int IdUsuario { get; set; }

        public string UsuarioResumen { get; set; } = "";

        public string DireccionEnvio { get; set; } = "";

        public string Estado { get; set; } = "Pendiente";

        // Líneas
        public List<CheckoutLineaVM> Lineas { get; set; } = new();

        // Totales
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }

        // Pago
        [Required(ErrorMessage = "Debe seleccionar un método de pago.")]
        public string MetodoPago { get; set; } = "Crédito"; // Tarjeta | Debito | Efectivo

        // Tarjeta/Débito
        [RequiredIfMetodoPago("Tarjeta", "Debito", ErrorMessage = "Debe ingresar el número de tarjeta.")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "El número de tarjeta debe tener 16 dígitos.")]
        public string? NumeroTarjeta { get; set; }

        [RequiredIfMetodoPago("Tarjeta", "Debito", ErrorMessage = "Debe ingresar la fecha de expiración.")]
        [RegularExpression(@"^\d{2}/\d{2}$", ErrorMessage = "Formato de expiración inválido (MM/AA).")]
        public string? ExpiracionMMYY { get; set; }

        [RequiredIfMetodoPago("Tarjeta", "Debito", ErrorMessage = "Debe ingresar el CVV.")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "El CVV debe tener 3 o 4 dígitos.")]
        public string? CVV { get; set; }

        [RequiredIfMetodoPago("Tarjeta", "Debito", ErrorMessage = "Debe ingresar el nombre del titular.")]
        public string? Titular { get; set; }

        // Efectivo
        [RequiredIfMetodoPago("Efectivo", ErrorMessage = "Debe ingresar el monto en efectivo.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto en efectivo debe ser mayor a 0.")]
        public decimal? MontoEfectivo { get; set; }
    }

    public class CheckoutLineaVM
    {
        public Guid LineId { get; set; }
        public int IdProducto { get; set; }
        public bool EsPersonalizado { get; set; }
        public string Nombre { get; set; } = "";
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; } = 1;
        public List<SeleccionCriterioDTO> Criterios { get; set; } = new();
    }
}
