using BelaEstilo.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.DTOs
{
    public class PedidoPersonalizadoCriterioDTO
    {

        public int Id { get; set; }

        public int IdPedidoPersonalizado { get; set; }

        public string NombreCriterio { get; set; } = null!;

        public string OpcionSeleccionada { get; set; } = null!;

        public decimal CostoExtra { get; set; }
        
        //solo para la vista/servicio
        public bool Seleccionado { get; set; }

        //public virtual PedidoPersonalizado IdPedidoPersonalizadoNavigation { get; set; } = null!;
    }
}
