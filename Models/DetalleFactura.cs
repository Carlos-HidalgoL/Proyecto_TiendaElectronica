using System.ComponentModel.DataAnnotations;

namespace Proyecto_TiendaElectronica.Models
{
    public class DetalleFactura
    {
        [Key]
        public int IdDetalleFactura { get; set; }

        public decimal SubTotal { get; set; }

        public decimal MontoTotal { get; set; }

    }
}
