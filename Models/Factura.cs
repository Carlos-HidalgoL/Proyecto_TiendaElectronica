using System.ComponentModel.DataAnnotations;

namespace Proyecto_TiendaElectronica.Models
{
    public class Factura
    {
        [Key]
        public int FacturaId { get; set; }
        public DateTime FechaCrecion { get; set; }

        public DateTime UltimaFechaImpresion { get; set; }

        public decimal SubTotal { get; set; }

        public decimal MontoTotal { get; set; }

        public string IdUsuario { get; set; }

        public Usuario Usuario { get; set; }   

                                                  
    }                                             
}                                                 
                                                  