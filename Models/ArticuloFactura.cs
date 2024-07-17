using System.ComponentModel.DataAnnotations;

namespace Proyecto_TiendaElectronica.Models
{
    public class ArticuloFactura
    {
        [Key]
        public int idArticulo { get; set; }

        public int idFactura { get; set; }

        public int CantidadArticulo { get; set; }

        public Articulo Articulo { get; set; }

        public Factura Factura { get; set; }
    }
}
