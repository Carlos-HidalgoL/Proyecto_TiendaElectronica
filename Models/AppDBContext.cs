﻿using Microsoft.EntityFrameworkCore;
using System;

namespace Proyecto_TiendaElectronica.Models
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }

        //Entities
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Factura> Factura { get; set; }
        public DbSet<Articulo> Articulo { get; set; }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<DetalleFactura> DetalleFactura { get; set; }
        public DbSet<ArticuloFactura> ArticuloFactura { get; set; }
        public DbSet<Imagen> Imagen { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                optionsBuilder.UseSqlServer("Server=localhost;Database=TiendaElectronica;Trusted_Connection=True;TrustServerCertificate=True;");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Cayo en el error: " + ex.ToString());
            }
        }
    }
}
