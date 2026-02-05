using System;
using System.Collections.Generic;

namespace BelaEstilo.Infraestructure.Models;

public partial class Resena
{
    public int IdResena { get; set; }

    public string? Comentario { get; set; }

    public int? Valoracion { get; set; }

    public DateTime? Fecha { get; set; }

    public int IdProducto { get; set; }

    public int IdUsuario { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
