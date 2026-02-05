using System;
using System.Collections.Generic;

namespace BelaEstilo.Infraestructure.Models;

public partial class ImagenProducto
{
    public int IdImagen { get; set; }

    public int IdProducto { get; set; }

    public byte[]? Imagen { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
