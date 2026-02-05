using System;
using System.Collections.Generic;

namespace BelaEstilo.Infraestructure.Models;

public partial class Pedido
{
    public int IdPedido { get; set; }

    public DateTime FechaPedido { get; set; }

    public string? Estado { get; set; }

    public decimal? Total { get; set; }

    public int IdUsuario { get; set; }

    public string? MetodoPago { get; set; }

    public string? DireccionEnvio { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<PedidoProducto> PedidoProducto { get; set; } = new List<PedidoProducto>();
}
