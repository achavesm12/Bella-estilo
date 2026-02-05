using System;
using System.Collections.Generic;

namespace BelaEstilo.Infraestructure.Models;

public partial class Categoria
{
    public int IdCategoria { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Producto> Producto { get; set; } = new List<Producto>();

    public virtual ICollection<Promocion> IdPromocion { get; set; } = new List<Promocion>();
}
