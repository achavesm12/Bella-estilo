using System;
using System.Collections.Generic;

namespace BelaEstilo.Infraestructure.Models;

public partial class Etiqueta
{
    public int IdEtiqueta { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Producto> IdProducto { get; set; } = new List<Producto>();
}
