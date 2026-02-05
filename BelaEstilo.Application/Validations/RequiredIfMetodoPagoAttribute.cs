using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.Validations
{
    public class RequiredIfMetodoPagoAttribute : ValidationAttribute
    {
        private readonly string[] _valoresEsperados;

        public RequiredIfMetodoPagoAttribute(params string[] valoresEsperados)
        {
            _valoresEsperados = valoresEsperados;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var metodoProp = validationContext.ObjectType.GetProperty("MetodoPago");
            if (metodoProp == null)
                return new ValidationResult($"La propiedad 'MetodoPago' no existe.");

            var metodoValor = metodoProp.GetValue(validationContext.ObjectInstance)?.ToString();

            if (_valoresEsperados.Contains(metodoValor, StringComparer.OrdinalIgnoreCase))
            {
                if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                {
                    return new ValidationResult(ErrorMessage ?? "Este campo es obligatorio para el método de pago seleccionado.");
                }
            }

            return ValidationResult.Success!;
        }
    }
}
