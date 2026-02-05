using System;
using System.Collections.Generic;

namespace BelaEstilo.Infraestructure.Models;

public partial class Carrito
{
    public int IdCarrito { get; set; }

    public DateTime FechaCreacion { get; set; }

    public int IdUsuario { get; set; }

    public virtual ICollection<CarritoProducto> CarritoProducto { get; set; } = new List<CarritoProducto>();

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
