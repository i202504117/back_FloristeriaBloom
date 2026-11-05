using back_FloristeriaBloom.Models;
using Microsoft.EntityFrameworkCore;

namespace back_FloristeriaBloom.Data
{
    public class FloristeriaDbContext : DbContext
    {
        public FloristeriaDbContext(DbContextOptions<FloristeriaDbContext> options) : base(options) { }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallesPedido { get; set; }

        // Nuevo: Adicionales y su relación con el detalle del pedido
        public DbSet<Adicional> Adicionales { get; set; }
        public DbSet<DetallePedidoAdicional> DetallesPedidoAdicionales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio).HasColumnType("decimal(10,2)");

            modelBuilder.Entity<DetallePedido>()
                .Property(d => d.PrecioUnitario).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<DetallePedido>()
                .Property(d => d.Subtotal).HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Total).HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Email).IsUnique();

            // Nuevo
            modelBuilder.Entity<Adicional>()
                .Property(a => a.Precio).HasColumnType("decimal(10,2)");

            modelBuilder.Entity<DetallePedidoAdicional>()
                .Property(d => d.PrecioUnitario).HasColumnType("decimal(10,2)");

            modelBuilder.Entity<DetallePedidoAdicional>()
                .HasOne(d => d.DetallePedido)
                .WithMany()
                .HasForeignKey(d => d.DetallePedidoId);

            modelBuilder.Entity<DetallePedidoAdicional>()
                .HasOne(d => d.Adicional)
                .WithMany()
                .HasForeignKey(d => d.AdicionalId);
        }
    }
}