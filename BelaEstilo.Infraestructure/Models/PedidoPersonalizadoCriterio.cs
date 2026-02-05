using System;
using System.Collections.Generic;

namespace BelaEstilo.Infraestructure.Models;

public partial class PedidoPersonalizadoCriterio
{
    public int Id { get; set; }

    public int IdPedidoPersonalizado { get; set; }

    public string NombreCriterio { get; set; } = null!;

    public string OpcionSeleccionada { get; set; } = null!;

    public decimal CostoExtra { get; set; }

    public virtual PedidoPersonalizado IdPedidoPersonalizadoNavigation { get; set; } = null!;
}
