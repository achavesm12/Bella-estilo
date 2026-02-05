using System;
using System.Collections.Generic;

namespace BelaEstilo.Infraestructure.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Contrasenna { get; set; } = null!;

    public string Rol { get; set; } = null!;

    public virtual ICollection<Carrito> Carrito { get; set; } = new List<Carrito>();

    public virtual ICollection<Pedido> Pedido { get; set; } = new List<Pedido>();

    public virtual ICollection<Resena> Resena { get; set; } = new List<Resena>();
}
