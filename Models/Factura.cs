namespace Proyecto_TiendaElectronica.Models
{
    public class Factura
    {
        public int IdFactura { get; set; }
        public DateTime FechaCrecion { get; set; }

        public DateTime UltimaFechaImpresion { get; set; }

        public string IdUsuario { get; set; }

        public int idDetalleFactura { get; set; }

                                                  
    }                                             
}                                                 
                                                  