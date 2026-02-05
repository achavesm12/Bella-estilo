using System;
using System.Collections.Generic;

namespace BelaEstilo.Infraestructure.Models;

public partial class Producto
{
    public int IdProducto { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal? Precio { get; set; }

    public int? Stock { get; set; }

    public string? TipoTela { get; set; }

    public string? TallaDisponible { get; set; }

    public bool? EstaActivo { get; set; }

    public int IdCategoria { get; set; }

    public virtual ICollection<CarritoProducto> CarritoProducto { get; set; } = new List<CarritoProducto>();

    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;

    public virtual ICollection<ImagenProducto> ImagenProducto { get; set; } = new List<ImagenProducto>();

    public virtual ICollection<PedidoProducto> PedidoProducto { get; set; } = new List<PedidoProducto>();

    public virtual ICollection<ProductoPersonalizacionOpcion> ProductoPersonalizacionOpcion { get; set; } = new List<ProductoPersonalizacionOpcion>();

    public virtual ICollection<Resena> Resena { get; set; } = new List<Resena>();

    public virtual ICollection<Etiqueta> IdEtiqueta { get; set; } = new List<Etiqueta>();

    public virtual ICollection<Promocion> IdPromocion { get; set; } = new List<Promocion>();
}
