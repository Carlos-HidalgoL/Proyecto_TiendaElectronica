using System.ComponentModel.DataAnnotations;

namespace Proyecto_TiendaElectronica.Models
{
    public class ArticuloFactura
    {
        [Key]
        public int idDetalleFactura { get; set; }

        public int idArticulo { get; set; }

        public int CantidadArticulo { get; set; }

        public DetalleFactura DetalleFactura { get; set; }

        public Articulo Articulo { get; set; }  
    }
}
