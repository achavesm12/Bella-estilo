using System;
using System.Collections.Generic;

namespace BelaEstilo.Infraestructure.Models;

public partial class Promocion
{
    public int IdPromocion { get; set; }

    public decimal? Descuento { get; set; }

    public DateOnly? FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Categoria> IdCategoria { get; set; } = new List<Categoria>();

    public virtual ICollection<Producto> IdProducto { get; set; } = new List<Producto>();
}
