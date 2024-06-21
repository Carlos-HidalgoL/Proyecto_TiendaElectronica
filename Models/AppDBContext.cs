using Microsoft.EntityFrameworkCore;
using System;

namespace Proyecto_TiendaElectronica.Models
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }

        //Entities
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<Articulo> Articulos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<DetalleFactura> DetalleFacturas { get; set; }
        public DbSet<ArticuloFactura> ArticulosFactura { get; set; }
        public DbSet<Imagen> Imagen { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                optionsBuilder.UseSqlServer("Server=localhost;Database=TiendaElectronica;Trusted_Connection=True;TrustServerCertificate=True;");
                Console.WriteLine("Hola a todos");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cayo en el error: " + ex.ToString());
            }
        }
    }
}
