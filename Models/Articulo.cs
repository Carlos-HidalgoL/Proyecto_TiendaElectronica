using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_TiendaElectronica.Models
{
    public class Articulo
    {
        [Key]
        public int ArticuloId { get; set; }
        public string Nombre { get; set; }
        public decimal  Precio { get; set; }
        public string Descripcion { get; set; }
        public string Marca { get; set; }
        public int Cantidad { get; set; }

        public int codigoImagen { get; set; }
        public int idCategoria { get; set; }

        
    }
}
