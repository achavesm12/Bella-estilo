using System;
using System.Collections.Generic;

namespace BelaEstilo.Infraestructure.Models;

public partial class PedidoPersonalizado
{
    public int IdPedidoPersonalizado { get; set; }

    public int IdPedido { get; set; }

    public int IdProducto { get; set; }

    public string NombreProductoPersonalizado { get; set; } = null!;

    public decimal CostoBase { get; set; }

    public decimal TotalProductoPersonalizado { get; set; }

    public virtual ICollection<PedidoPersonalizadoCriterio> PedidoPersonalizadoCriterio { get; set; } = new List<PedidoPersonalizadoCriterio>();
}
