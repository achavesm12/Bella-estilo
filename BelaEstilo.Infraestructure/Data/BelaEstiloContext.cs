using System;
using System.Collections.Generic;
using BelaEstilo.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace BelaEstilo.Infraestructure.Data;

public partial class BelaEstiloContext : DbContext
{
    public BelaEstiloContext(DbContextOptions<BelaEstiloContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Carrito> Carrito { get; set; }

    public virtual DbSet<CarritoProducto> CarritoProducto { get; set; }

    public virtual DbSet<Categoria> Categoria { get; set; }

    public virtual DbSet<Etiqueta> Etiqueta { get; set; }

    public virtual DbSet<ImagenProducto> ImagenProducto { get; set; }

    public virtual DbSet<Pedido> Pedido { get; set; }

    public virtual DbSet<PedidoPersonalizado> PedidoPersonalizado { get; set; }

    public virtual DbSet<PedidoPersonalizadoCriterio> PedidoPersonalizadoCriterio { get; set; }

    public virtual DbSet<PedidoProducto> PedidoProducto { get; set; }

    public virtual DbSet<Producto> Producto { get; set; }

    public virtual DbSet<ProductoPersonalizacionOpcion> ProductoPersonalizacionOpcion { get; set; }

    public virtual DbSet<Promocion> Promocion { get; set; }

    public virtual DbSet<Resena> Resena { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Carrito>(entity =>
        {
            entity.HasKey(e => e.IdCarrito).HasName("PK__Carrito__8B4A618C8E96BC32");

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Carrito)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Carrito__IdUsuar__3F466844");
        });

        modelBuilder.Entity<CarritoProducto>(entity =>
        {
            entity.HasKey(e => new { e.IdCarrito, e.IdProducto }).HasName("PK__CarritoP__9BD2E8AD0D79C97D");

            entity.HasOne(d => d.IdCarritoNavigation).WithMany(p => p.CarritoProducto)
                .HasForeignKey(d => d.IdCarrito)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CarritoPr__IdCar__4222D4EF");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.CarritoProducto)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CarritoPr__IdPro__4316F928");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK__Categori__A3C02A102E651E43");

            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<Etiqueta>(entity =>
        {
            entity.HasKey(e => e.IdEtiqueta).HasName("PK__Etiqueta__5041D723916EDD35");

            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<ImagenProducto>(entity =>
        {
            entity.HasKey(e => e.IdImagen).HasName("PK__ImagenPr__B42D8F2A9ADB013D");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.ImagenProducto)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Imagen_Producto");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.IdPedido).HasName("PK__Pedido__9D335DC35130B78A");

            entity.Property(e => e.DireccionEnvio).HasMaxLength(255);
            entity.Property(e => e.Estado).HasMaxLength(50);
            entity.Property(e => e.FechaPedido)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MetodoPago).HasMaxLength(50);
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Pedido)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pedido__IdUsuari__46E78A0C");
        });

        modelBuilder.Entity<PedidoPersonalizado>(entity =>
        {
            entity.HasKey(e => e.IdPedidoPersonalizado).HasName("PK__PedidoPe__331B337663DCCA54");

            entity.Property(e => e.CostoBase).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NombreProductoPersonalizado).HasMaxLength(100);
            entity.Property(e => e.TotalProductoPersonalizado).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<PedidoPersonalizadoCriterio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PedidoPe__3214EC070887BB85");

            entity.Property(e => e.CostoExtra).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.NombreCriterio).HasMaxLength(100);
            entity.Property(e => e.OpcionSeleccionada).HasMaxLength(100);

            entity.HasOne(d => d.IdPedidoPersonalizadoNavigation).WithMany(p => p.PedidoPersonalizadoCriterio)
                .HasForeignKey(d => d.IdPedidoPersonalizado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PedidoPersonalizadoCriterio_PedidoPersonalizado");
        });

        modelBuilder.Entity<PedidoProducto>(entity =>
        {
            entity.HasKey(e => new { e.IdPedido, e.IdProducto }).HasName("PK__PedidoPr__8DABD4E24CF1A49D");

            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdPedidoNavigation).WithMany(p => p.PedidoProducto)
                .HasForeignKey(d => d.IdPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PedidoPro__IdPed__49C3F6B7");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.PedidoProducto)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PedidoPro__IdPro__4AB81AF0");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__Producto__09889210FB940F73");

            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TallaDisponible).HasMaxLength(20);
            entity.Property(e => e.TipoTela).HasMaxLength(50);

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Producto)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Producto_Categoria");

            entity.HasMany(d => d.IdEtiqueta).WithMany(p => p.IdProducto)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductoEtiqueta",
                    r => r.HasOne<Etiqueta>().WithMany()
                        .HasForeignKey("IdEtiqueta")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ProductoE__IdEti__30F848ED"),
                    l => l.HasOne<Producto>().WithMany()
                        .HasForeignKey("IdProducto")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ProductoE__IdPro__300424B4"),
                    j =>
                    {
                        j.HasKey("IdProducto", "IdEtiqueta").HasName("PK__Producto__2C8C8F62D3C281ED");
                    });

            entity.HasMany(d => d.IdPromocion).WithMany(p => p.IdProducto)
                .UsingEntity<Dictionary<string, object>>(
                    "PromocionProducto",
                    r => r.HasOne<Promocion>().WithMany()
                        .HasForeignKey("IdPromocion")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Promocion__IdPro__36B12243"),
                    l => l.HasOne<Producto>().WithMany()
                        .HasForeignKey("IdProducto")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Promocion__IdPro__35BCFE0A"),
                    j =>
                    {
                        j.HasKey("IdProducto", "IdPromocion").HasName("PK__Promocio__4AD7FE3A967E7065");
                    });
        });

        modelBuilder.Entity<ProductoPersonalizacionOpcion>(entity =>
        {
            entity.HasKey(e => e.IdOpcion).HasName("PK__Producto__4F238858C07E5D47");

            entity.HasIndex(e => new { e.IdProducto, e.Activo }, "IX_ProductoPersonalizacionOpcion_IdProducto_Activo");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CostoExtra).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.NombreCriterio).HasMaxLength(100);

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.ProductoPersonalizacionOpcion)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProdPersOpcion_Producto");
        });

        modelBuilder.Entity<Promocion>(entity =>
        {
            entity.HasKey(e => e.IdPromocion).HasName("PK__Promocio__35F6C2A5B4835F09");

            entity.Property(e => e.Descuento).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Nombre).HasMaxLength(100);

            entity.HasMany(d => d.IdCategoria).WithMany(p => p.IdPromocion)
                .UsingEntity<Dictionary<string, object>>(
                    "PromocionCategoria",
                    r => r.HasOne<Categoria>().WithMany()
                        .HasForeignKey("IdCategoria")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Promocion__IdCat__5DCAEF64"),
                    l => l.HasOne<Promocion>().WithMany()
                        .HasForeignKey("IdPromocion")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Promocion__IdPro__5CD6CB2B"),
                    j =>
                    {
                        j.HasKey("IdPromocion", "IdCategoria").HasName("PK__Promocio__2FCAC004E3F82F54");
                    });
        });

        modelBuilder.Entity<Resena>(entity =>
        {
            entity.HasKey(e => e.IdResena).HasName("PK__Resena__A53BB7F8017042A5");

            entity.Property(e => e.Comentario).HasMaxLength(500);
            entity.Property(e => e.Fecha).HasColumnType("datetime");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Resena)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Resena__IdProduc__3A81B327");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Resena)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Resena__IdUsuari__3B75D760");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__5B65BF970734258E");

            entity.Property(e => e.Contrasenna).HasMaxLength(255);
            entity.Property(e => e.Correo).HasMaxLength(100);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Rol).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
