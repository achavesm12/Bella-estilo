using System;
using System.Collections.Generic;

namespace BelaEstilo.Infraestructure.Models;

public partial class ProductoPersonalizacionOpcion
{
    public int IdOpcion { get; set; }

    public int IdProducto { get; set; }

    public string NombreCriterio { get; set; } = null!;

    public decimal CostoExtra { get; set; }

    public bool Activo { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
